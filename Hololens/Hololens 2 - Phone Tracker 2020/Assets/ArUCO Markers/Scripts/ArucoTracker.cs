﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;
using TMPro;
#if ENABLE_WINMD_SUPPORT
using Windows.Graphics.Imaging;
using Windows.Perception.Spatial;
#endif

public class ArucoTracker : MonoBehaviour
{
    public TMP_Text status;
    public MediaCaptureUtility.MediaCaptureProfiles mediaProfile;
    public ArUcoUtils.ArUcoDictionaryName ArUcoDictionaryName = ArUcoUtils.ArUcoDictionaryName.DICT_6X6_50;
    public ArUcoUtils.ArUcoTrackingType ArUcoTrackingType = ArUcoUtils.ArUcoTrackingType.Markers;
    public ArUcoBoardPositions boardPositions;

    public Action<IReadOnlyList<Marker>> onDetectionFinished;

    private List<Marker> markersInUnity = new List<Marker>();
    private MediaCaptureUtility _MediaCaptureUtility;
    private bool _isRunning = false;

    private Queue<Vector3> _posCamQ = new Queue<Vector3>();
    private Queue<Quaternion> _rotCamQ = new Queue<Quaternion>();

    private Queue<Transform> instantiatedPrefabs = new Queue<Transform>();
#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// OpenCV windows runtime dll component
    /// </summary>
    OpenCVRuntimeComponent.CvUtils CvUtils;
    OpenCVRuntimeComponent.CameraCalibrationParams calibParams;

    /// <summary>
    /// Coordinate system reference for Unity to WinRt transform construction.
    /// </summary>
    private SpatialCoordinateSystem _unityCoordinateSystem = null;
    private SpatialCoordinateSystem _frameCoordinateSystem = null;

#endif
    async void Start()
    {
        try
        {
#if ENABLE_WINMD_SUPPORT

            // Asynchronously start media capture
            await StartMediaCapture();

            // Configure the dll with input parameters
            CvUtils = new OpenCVRuntimeComponent.CvUtils(
                boardPositions.ComputeMarkerSizeForTrackingType(
                    ArUcoTrackingType, 
                    boardPositions.markerSizeForSingle,
                    boardPositions.markerSizeForBoard),
                boardPositions.numMarkers,
                (int)ArUcoDictionaryName,
                boardPositions.FillCustomObjectPointsFromUnity());
            Debug.Log("Created new instance of the cvutils class.");

            // Run processing loop in separate parallel Task, get the latest frame
            // and asynchronously evaluate
            Debug.Log("Begin tracking in frame grab loop.");
            _isRunning = true;
            _MediaCaptureUtility.onFrameArrived += HandleArUcoTracking;

            // Run the frame grab and aruco tracking in a new task block
            //await Task.Run(() =>
            //{
            //    while (_isRunning)
            //    {
            //        if (_MediaCaptureUtility.IsCapturing)
            //        {
            //            var mediaFrameReference = _MediaCaptureUtility.GetLatestFrame();
            //            HandleArUcoTracking(mediaFrameReference);
            //            mediaFrameReference?.Dispose();
            //        }
            //        else
            //        {
            //            return;
            //        }
            //    }
            //});
#endif 
        }
        catch (Exception ex)
        {
            status.text = $"Error init: {ex.Message}";
            Debug.LogError($"Failed to start marker tracking: {ex}");
        }
    }

    private async void OnDestroy()
    {
        _isRunning = false;
        if (_MediaCaptureUtility != null)
        {
            await _MediaCaptureUtility.StopMediaFrameReaderAsync();
        }
    }

    private async Task StartMediaCapture()
    {

        status.text = $"Starting Media Capture";

#if ENABLE_WINMD_SUPPORT
        // Configure camera to return frames fitting the model input size
        try
        {
            Debug.Log("Creating MediaCaptureUtility and initializing frame reader.");
            _MediaCaptureUtility = new MediaCaptureUtility();
            await _MediaCaptureUtility.InitializeMediaFrameReaderAsync(mediaProfile);
            status.text = $"Camera started. Running!";
            Debug.Log("Successfully initialized frame reader.");
        }
        catch (Exception ex)
        {
            status.text = $"Failed to start media capture: {ex.Message}. Using loaded/picked image.";
        }

        // Get the unity spatial coordinate system
        try
        {
            _unityCoordinateSystem = PerceptionInterop.GetSceneCoordinateSystem(Pose.identity) as SpatialCoordinateSystem;
            Debug.Log("Successfully cached pointer to Unity spatial coordinate system.");
        }
        catch (Exception ex)
        {
            status.text = $"Failed to get Unity spatial coordinate system: {ex.Message}.";
        }
#endif
    }

#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// Method to extract important paramters and software bitmap from
    /// media frame reference and send to opencv dll for marker-based or
    /// board-based tracking.
    /// </summary>
    private void HandleArUcoTracking(Windows.Media.Capture.Frames.MediaFrameReference mediaFrameReference)
    {
        // Request software bitmap from media frame reference
        var videoMediaFrame = mediaFrameReference?.VideoMediaFrame;
        var softwareBitmap = videoMediaFrame?.SoftwareBitmap;
        
        if (softwareBitmap == null)
            return;
            
        // Cache the current camera projection transform (not using currently) ??
        //var cameraProjectionTransform = camIntrinsics.UndistortedProjectionTransform;
            
        // Cache the current camera frame coordinate system
        _frameCoordinateSystem = mediaFrameReference.CoordinateSystem;
            
        if (calibParams == null)
        {
            var camIntrinsics = videoMediaFrame.CameraIntrinsics;
            calibParams = new OpenCVRuntimeComponent.CameraCalibrationParams(
                    camIntrinsics.FocalLength, // Focal length
                    camIntrinsics.PrincipalPoint, // Principal point
                    camIntrinsics.RadialDistortion, // Radial distortion
                    camIntrinsics.TangentialDistortion, // Tangential distortion
                    (int)camIntrinsics.ImageWidth, // Image width
                    (int)camIntrinsics.ImageHeight); // Image height
        }

        softwareBitmap.Dispose();
        return;

        switch (ArUcoTrackingType)
        {
            case ArUcoUtils.ArUcoTrackingType.Markers:
                var markers = DetectMarkers(softwareBitmap, calibParams);
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    onDetectionFinished?.Invoke(markers);
                }, false);
                break;

            case ArUcoUtils.ArUcoTrackingType.CustomBoard:
                markers = DetectBoard(softwareBitmap, calibParams);
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    onDetectionFinished?.Invoke(markers);
                }, false);
                break;

            case ArUcoUtils.ArUcoTrackingType.None:
                status.text = $"Not running tracking...";
                break;

            default:
                status.text = $"No option selected for tracking...";
                break;
        }

        softwareBitmap.Dispose();
    }

    private IReadOnlyList<Marker> DetectMarkers(SoftwareBitmap softwareBitmap, OpenCVRuntimeComponent.CameraCalibrationParams calibParams)
    {

        // Get marker detections from opencv component
        var markers = CvUtils.DetectMarkers(softwareBitmap, calibParams);
        markersInUnity.Clear();

        // Iterate across detections
        foreach (var marker in markers)
        {
            var transformUnityCamera = ArUcoUtils.GetTransformInUnityCamera(
                        ArUcoUtils.Vec3FromFloat3(marker.Position),
                        ArUcoUtils.RotationQuatFromRodrigues(ArUcoUtils.Vec3FromFloat3(marker.Rotation)));

            // Camera view transform used for transform chain
            var cameraToWorld = GetViewToUnityTransform(_frameCoordinateSystem);
            
            var transformUnityWorld = cameraToWorld * transformUnityCamera;

            var posInUnity = ArUcoUtils.GetVectorFromMatrix(transformUnityWorld);
            var rotInUnity = ArUcoUtils.GetQuatFromMatrix(transformUnityWorld);
            var markerInUnity = new Marker(posInUnity, rotInUnity);
            markersInUnity.Add(markerInUnity);
        }

        return markersInUnity;
    }

    private IReadOnlyList<Marker> DetectBoard(SoftwareBitmap softwareBitmap, OpenCVRuntimeComponent.CameraCalibrationParams calibParams)
    {
        // Get marker detections from opencv component
        var board = CvUtils.DetectBoard(softwareBitmap, calibParams);
        markersInUnity.Clear();

        Vector3 unityPosition = Vector3.zero, markerPosition = Vector3.zero;
        Quaternion unityRotation = Quaternion.identity, markerRotation = Quaternion.identity;
        var isDetected = board.IsDetected;
        if (isDetected)
        {
            markerPosition = ArUcoUtils.Vec3FromFloat3(board.Position);
            markerRotation = ArUcoUtils.RotationQuatFromRodrigues(ArUcoUtils.Vec3FromFloat3(board.Rotation));

            // Get the transform from C++ component and format for Unity coordinate system
            var transformUnityCamera = ArUcoUtils.GetTransformInUnityCamera(
                markerPosition,
                markerRotation);

            // Camera view transform used for transform chain
            var cameraToWorld = GetViewToUnityTransform(_frameCoordinateSystem);
            
            var predefinedMatrix = Matrix4x4.identity;
            var transformUnityWorld = cameraToWorld * transformUnityCamera;
            unityPosition = ArUcoUtils.GetVectorFromMatrix(transformUnityWorld);
            unityRotation = ArUcoUtils.GetQuatFromMatrix(transformUnityWorld);
            
            var markerInUnity = new Marker(unityPosition, unityRotation);
            markersInUnity.Add(markerInUnity);
        }

        return markersInUnity;
    }

    //https://github.com/microsoft/MixedReality-SpectatorView/blob/7796da6acb0ae41bed1b9e0e9d1c5c683b4b8374/src/SpectatorView.Unity/Assets/PhotoCapture/Scripts/HoloLensCamera.cs#L1256
    /// <summary>
    /// Create the camera extrinsics from unity coordinate system
    /// and the current frame coordinate system.
    /// </summary>
    /// <param name="frameCoordinateSystem"></param>
    /// <returns></returns>
    private Matrix4x4 GetViewToUnityTransform(
        SpatialCoordinateSystem frameCoordinateSystem)
    {
        if (frameCoordinateSystem == null || _unityCoordinateSystem == null)
        {
            return Matrix4x4.identity;
        }

        // Get the reference transform from camera frame to unity space
        System.Numerics.Matrix4x4? cameraToUnityRef = frameCoordinateSystem.TryGetTransformTo(_unityCoordinateSystem);

        // Return identity if value does not exist
        if (!cameraToUnityRef.HasValue)
            return Matrix4x4.identity;

        // No cameraViewTransform availabnle currently, using identity for HL2
        // Inverse of identity is identity
        var viewToCamera = Matrix4x4.identity;
        var cameraToUnity = ArUcoUtils.Mat4x4FromFloat4x4(cameraToUnityRef.Value);

        // Compute transform to relate winrt coordinate system with unity coordinate frame (viewToUnity)
        // WinRT transfrom -> Unity transform by transpose and flip row 3
        var viewToUnityWinRT = viewToCamera * cameraToUnity;
        var viewToUnity = Matrix4x4.Transpose(viewToUnityWinRT);
        viewToUnity.m20 *= -1.0f;
        viewToUnity.m21 *= -1.0f;
        viewToUnity.m22 *= -1.0f;
        viewToUnity.m23 *= -1.0f;

        return viewToUnity;
    }
#endif
}
