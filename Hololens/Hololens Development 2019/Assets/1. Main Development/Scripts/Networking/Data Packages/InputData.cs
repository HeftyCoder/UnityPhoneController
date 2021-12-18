﻿using System;
using System.Collections.Generic;
using Barebones.Networking;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
public class InputData : SerializablePacket
{
    public DeviceDescription deviceDescription;
    public InputDeviceChange deviceChange;
    public BaseInput inputData;

    public InputData() { }
    public InputData(DeviceDescription desc) => deviceDescription = desc;

    public override void ToBinaryWriter(EndianBinaryWriter writer)
    {
        writer.Write(deviceDescription);
        writer.Write((int)deviceChange);
        writer.Write(inputData);
    }
    public override void FromBinaryReader(EndianBinaryReader reader)
    {
        deviceDescription = new DeviceDescription();
        reader.ReadPacket(deviceDescription);
        deviceChange = (InputDeviceChange)reader.ReadInt32();

        var layout = deviceDescription.Layout;
        inputData = InputFactory.CreateInput(layout);
        reader.ReadPacket(inputData);
    }

}
