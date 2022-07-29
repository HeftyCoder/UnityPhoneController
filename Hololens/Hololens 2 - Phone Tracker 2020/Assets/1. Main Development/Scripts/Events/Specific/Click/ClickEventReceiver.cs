﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Barebones.Networking;
using UnityEngine.Events;
public class ClickEventReceiver : MonoBehaviour
{
    [SerializeField] EventManager manager;
    [SerializeField] EventIdentifier identifier;

    [SerializeField] UnityEvent<int> clickedSetInEditor;
    public event Action<ClickEvent> clicked;
    private void Awake()
    {
        manager.SetEventHandler(identifier, OnClickArrived);
    }

    private void OnClickArrived(IIncommingMessage message)
    {
        var click = new ClickEvent();
        message.Deserialize(click);
        clicked?.Invoke(click);
        clickedSetInEditor?.Invoke(click.clickCount);
    }
}
