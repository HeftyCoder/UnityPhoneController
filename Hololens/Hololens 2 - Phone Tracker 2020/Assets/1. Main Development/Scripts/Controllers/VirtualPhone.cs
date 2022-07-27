using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class VirtualPhone : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text status;
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] PhoneServer phoneServer;

    InputActions inputs => phoneServer.InputActions;

    [Header("VIO Reported")]
    public Vector3 posVio;
    public Quaternion rotVio;

    [Header("Marker and At Marker Vio")]
    public Vector3 posMarker;
    public Quaternion rotMarker;

    private Quaternion R = Quaternion.identity;
    private Vector3 T = Vector3.zero;

    private InputAction phonePosition, phoneRotation;

    [ContextMenu("Test a matrix rotation")]
    private void TestMatrixRotation()
    {
#if UNITY_EDITOR
        Undo.RecordObject(transform, "Undo Transformation");
#endif
        var m = transform.worldToLocalMatrix;
        m = m * Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0)) * m.inverse;
        var pos = m.GetColumn(3);
        var rot = m.rotation;

        transform.localPosition = pos;
        transform.localRotation = rot;
    }
    [ContextMenu("Test Calculate RT")]
    private void TestInEditor()
    {
        CalculateRT(posVio, rotVio, posMarker, rotMarker);
    }
    private void Awake()
    {
        var actions = inputs.Phone;
        posMarker = transform.position;
        rotMarker = transform.rotation;

        posVio = transform.position;
        rotVio = transform.rotation;

        //Reading the values that the tracker has for us
        phonePosition = actions.PhonePosition;
        phoneRotation = actions.PhoneRotation;
    }
    private void OnEnable()
    {
        phonePosition.performed += ReadPosition;
        phoneRotation.performed += ReadRotation;
        arucoTracker.onDetectionFinished += OnArucoScanFinished;
    }
    private void OnDisable()
    {
        phonePosition.performed -= ReadPosition;
        phoneRotation.performed -= ReadRotation;
        arucoTracker.onDetectionFinished -= OnArucoScanFinished;
    }
    private void ReadPosition(InputAction.CallbackContext ctx) => posVio = ctx.ReadValue<Vector3>();
    private void ReadRotation(InputAction.CallbackContext ctx) => rotVio = ctx.ReadValue<Quaternion>();

    private void OnArucoScanFinished(IReadOnlyList<Marker> markers)
    {
        if (markers.Count == 0)
            return;
        //We're treating this as if it was a board
        var marker = markers[0];
        posMarker = marker.position;
        rotMarker = marker.rotation;

        transform.SetPositionAndRotation(marker.position, marker.rotation);
        var actions = inputs.Phone;

        //Values reported from VIO at the moment of aruco detection
        posVio = actions.PhonePosition.ReadValue<Vector3>();
        rotVio = actions.PhoneRotation.ReadValue<Quaternion>();

        CalculateRT(posVio, rotVio, marker.position, marker.rotation);
    }

    private void CalculateRT(Vector3 posVio, Quaternion rotVio, Vector3 posMarker, Quaternion rotMarker)
    {
        R = Quaternion.Inverse(rotVio) * rotMarker;
        T = posMarker - (R * posVio);
    }
    private void Update()
    {
        var rot = R * rotVio;
        var pos = T + R * posVio;

        status.text = rot.eulerAngles.ToString();
        transform.SetPositionAndRotation(pos, rot);
    }
}
