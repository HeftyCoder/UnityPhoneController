﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class DeviceServer : MonoBehaviour
{
    public static DeviceServer Instance;

    [SerializeField] TransportServerSocket server;
    [SerializeField] int targetFramerate = -1;
    [SerializeField] int port = 5000;
    [SerializeField] bool onStart = false;
    [SerializeField] ArucoTracker arucoTracker;
    [SerializeField] bool enableArucoTrackerOnConnection = false;

    [SerializeField] private ServerNetworkClock clock = new ServerNetworkClock();
    private bool listening = false;
    private int count = 0;
    private DeviceClient localClient;
    public IServerSocket ServerSocket => server;

    private InputActions inputActions;
    private Dictionary<IPeer, Dictionary<string, InputDevice>> peerToDevices = new Dictionary<IPeer, Dictionary<string,InputDevice>>();

    //These should probably be per peer - dont care about it right now
    private double lastNetworkTimeStamp, latency;
    //
    public double LastNetworkTimestamp => lastNetworkTimeStamp;
    public double Latency => latency;
    public ServerNetworkClock Clock => clock;
    public InputActions InputActions => inputActions;
    public HashSet<InputDevice> CreatedDevices { get; private set; } = new HashSet<InputDevice>();
    public bool haveDevicesChanged = false;
    public Dictionary<short, Action<IIncommingMessage>> Operations { get; private set; } = new Dictionary<short, Action<IIncommingMessage>>();
    private void Awake()
    {
        clock.Reset();
        inputActions = new InputActions();
        RefreshDevices();
        localClient = GetComponent<DeviceClient>();
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        Instance = this;
    }
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
    private void Start()
    {
        Application.targetFrameRate = targetFramerate;
        InitializeServer();
        if (onStart)
            Listen();
    }
    private void OnDestroy()
    {
        foreach (var device in CreatedDevices)
            InputSystem.RemoveDevice(device);
    }

    private void InitializeServer()
    {
        Operations.Add((short)global::Operations.Subscribe, this.OnSubscribe);
        Operations.Add((short)global::Operations.StateData, this.OnPhoneData);

        ServerSocket.Connected += (peer) =>
        {
            count++;
            var serverDevices = new Dictionary<string, InputDevice>();
            peerToDevices.Add(peer, serverDevices);
            Debug.Log($"Connected :{count}");

            peer.MessageReceived += (message) =>
            {
                var opcode = message.OpCode;
                Operations[opcode].Invoke(message);
            };
        };

        ServerSocket.Disconnected += (peer) =>
        {
            count--;
            Debug.Log($"Diconnected: {count}");
            Clear(peer);
        };
    }
    public void EnableTracker(bool enable) => arucoTracker.enabled = enable;
    public void Listen()
    {
        Listen(port);
    }

    public void Listen(int port)
    {
        if (listening)
            return;
        listening = true;
        ServerSocket.Listen(port);
    }

    #region Operations
    private void OnSubscribe(IIncommingMessage message)
    {
        var subData = new SubscriptionData();
        message.Deserialize(subData);
 
        arucoTracker.boardPositions.arucoLayout = subData.arucoLayout;
        
        if (enableArucoTrackerOnConnection)
            arucoTracker.enabled = true;

        var peer = message.Peer;
        foreach (var desc in subData.devices)
        {
            AddDevice(desc, peer);
        }

        RefreshDevices();
    }
    private void OnPhoneData(IIncommingMessage message)
    {
        var phoneData = new DeviceData();
        message.Deserialize(phoneData);
        var peer = message.Peer;

        ProcessPhoneData(peer, phoneData);

        //Update time
        var pong = clock.GetPongMessage(phoneData.ping);
        message.Respond(pong, ResponseStatus.Success);

        //Debug.Log($"Time of Server: {clock.Time} \nTime of Phone at message: {phoneData.networkTimestamp} \nLatency: {phoneData.latency}");
        lastNetworkTimeStamp = phoneData.networkTimestamp;
        latency = phoneData.latency;
    }

    private void ProcessPhoneData(IPeer peer, DeviceData phoneData)
    {
        foreach (var data in phoneData.inputDatas)
        {
            var desc = data.deviceDescription;
            switch (data.deviceChange)
            {
                case InputDeviceChange.Added:
                    haveDevicesChanged = true;
                    var existingDevice = GetDevice(desc.Layout, peer);
                    if (existingDevice != null)
                    {
                        Debug.Log($"There's already a device with {desc.Layout}");
                        return;
                    }
                    Debug.Log(desc.device);
                    AddDevice(desc, peer);
                    break;
                case InputDeviceChange.Removed:
                    haveDevicesChanged = true;
                    RemoveDevice(data.deviceDescription, peer);
                    continue;
            }

            var input = data.inputData;
            var device = GetDevice(desc.Layout, peer);
            input.QueueInput(device);
        }
        if (haveDevicesChanged)
            RefreshDevices();

        foreach (var ev in phoneData.events)
        {
            var message = MessageHelper.FromBytes(ev.bytes, 0, peer);
            var opcode = message.OpCode;
            if (Operations.TryGetValue(opcode, out var result))
                result?.Invoke(message);
        }
    }
    #endregion

    #region Adding-Removing Devices
    private void AddDevice(DeviceDescription desc, IPeer peer)
    {
        var name = $"{desc.CustomName}_{peer.Id}";
        var layout = desc.Layout;
        localClient?.SetCaptureEvents(false);
        var device = InputSystem.AddDevice(layout, name);
        localClient?.SetCaptureEvents(true);
        var peerDevices = peerToDevices[peer];
        peerDevices.Add(layout, device);
        Debug.Log($"{name} {device}");
        CreatedDevices.Add(device);
    }
    private void RemoveDevice(DeviceDescription desc, IPeer peer)
    {
        localClient?.SetCaptureEvents(false);

        var layout = desc.Layout;
        var peerDevices = peerToDevices[peer];
        if (!peerDevices.TryGetValue(layout, out var device))
            return;
        peerDevices.Remove(layout);
        CreatedDevices.Remove(device);
        InputSystem.RemoveDevice(device);

        localClient?.SetCaptureEvents(true);
    }

    private InputDevice GetDevice(string layout, IPeer peer)
    {
        var peerDevices = peerToDevices[peer];
        if (peerDevices.TryGetValue(layout, out InputDevice device))
            return device;
        return null;
    }

    private void Clear(IPeer peer)
    {
        haveDevicesChanged = true;
        localClient?.SetCaptureEvents(false);
        
        foreach (var device in peerToDevices[peer].Values)
        {
            CreatedDevices.Remove(device);
            InputSystem.RemoveDevice(device);
        }

        peerToDevices.Remove(peer);
        localClient?.SetCaptureEvents(true);
        RefreshDevices();
    }

    private void RefreshDevices()
    {
        var createdDevices = CreatedDevices;
        var devices = new InputDevice[createdDevices.Count];

        int i = 0;
        foreach (var device in createdDevices)
        {
            devices[i] = device;
            i++;
        }

        inputActions.devices = new ReadOnlyArray<InputDevice>(devices);
    }
    #endregion
    
}
