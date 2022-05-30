//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/1. Main Development/Input Actions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input Actions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""ac1219ed-a02c-4678-9d87-8466f3869e14"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""a64ee366-fd8f-4342-a8bc-f657c758b3e2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Attitude"",
                    ""type"": ""Value"",
                    ""id"": ""da088950-1be8-422d-a02b-d0f23f7837a0"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LinearAcceleration"",
                    ""type"": ""Value"",
                    ""id"": ""b5453b4b-db94-42eb-96c1-486578c13cf3"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Acceleration"",
                    ""type"": ""Value"",
                    ""id"": ""c6a5be75-b523-474e-b8b1-1926b4bd8f80"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""AngVelocity"",
                    ""type"": ""Value"",
                    ""id"": ""7e8ba50d-49a1-435a-943c-b92a54685bb4"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PhonePosition"",
                    ""type"": ""Value"",
                    ""id"": ""9e4923e7-5df7-429d-a284-89c692995873"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PhoneRotation"",
                    ""type"": ""Value"",
                    ""id"": ""6b5b6bc3-ab64-445d-ba22-10b0d4fedaaa"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""65055960-6e9a-4502-b3b2-1a75e17b3fba"",
                    ""path"": ""<AttitudeSensor>/attitude"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attitude"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cdd563f2-d436-4c0a-8f0a-b453d633aac1"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34068a5e-aa1a-43dc-a88d-020197052661"",
                    ""path"": ""<LinearAccelerationSensor>/acceleration"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LinearAcceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cee76cdc-2631-4a81-8045-c7dcde866ebc"",
                    ""path"": ""<Accelerometer>/acceleration"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f55d2ab8-f479-4e81-bbc5-1cd2c8668b8c"",
                    ""path"": ""<Gyroscope>/angularVelocity"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AngVelocity"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""594abc7e-9796-42ae-9ede-f01f749d662b"",
                    ""path"": ""<TrackedDevice>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PhonePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03940047-4fa6-4e3f-92af-769c55878f1d"",
                    ""path"": ""<TrackedDevice>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PhoneRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Attitude = m_Player.FindAction("Attitude", throwIfNotFound: true);
        m_Player_LinearAcceleration = m_Player.FindAction("LinearAcceleration", throwIfNotFound: true);
        m_Player_Acceleration = m_Player.FindAction("Acceleration", throwIfNotFound: true);
        m_Player_AngVelocity = m_Player.FindAction("AngVelocity", throwIfNotFound: true);
        m_Player_PhonePosition = m_Player.FindAction("PhonePosition", throwIfNotFound: true);
        m_Player_PhoneRotation = m_Player.FindAction("PhoneRotation", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Attitude;
    private readonly InputAction m_Player_LinearAcceleration;
    private readonly InputAction m_Player_Acceleration;
    private readonly InputAction m_Player_AngVelocity;
    private readonly InputAction m_Player_PhonePosition;
    private readonly InputAction m_Player_PhoneRotation;
    public struct PlayerActions
    {
        private @InputActions m_Wrapper;
        public PlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Attitude => m_Wrapper.m_Player_Attitude;
        public InputAction @LinearAcceleration => m_Wrapper.m_Player_LinearAcceleration;
        public InputAction @Acceleration => m_Wrapper.m_Player_Acceleration;
        public InputAction @AngVelocity => m_Wrapper.m_Player_AngVelocity;
        public InputAction @PhonePosition => m_Wrapper.m_Player_PhonePosition;
        public InputAction @PhoneRotation => m_Wrapper.m_Player_PhoneRotation;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Attitude.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttitude;
                @Attitude.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttitude;
                @Attitude.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttitude;
                @LinearAcceleration.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLinearAcceleration;
                @LinearAcceleration.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLinearAcceleration;
                @LinearAcceleration.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLinearAcceleration;
                @Acceleration.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAcceleration;
                @Acceleration.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAcceleration;
                @Acceleration.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAcceleration;
                @AngVelocity.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAngVelocity;
                @AngVelocity.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAngVelocity;
                @AngVelocity.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAngVelocity;
                @PhonePosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPhonePosition;
                @PhonePosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPhonePosition;
                @PhonePosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPhonePosition;
                @PhoneRotation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPhoneRotation;
                @PhoneRotation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPhoneRotation;
                @PhoneRotation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPhoneRotation;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Attitude.started += instance.OnAttitude;
                @Attitude.performed += instance.OnAttitude;
                @Attitude.canceled += instance.OnAttitude;
                @LinearAcceleration.started += instance.OnLinearAcceleration;
                @LinearAcceleration.performed += instance.OnLinearAcceleration;
                @LinearAcceleration.canceled += instance.OnLinearAcceleration;
                @Acceleration.started += instance.OnAcceleration;
                @Acceleration.performed += instance.OnAcceleration;
                @Acceleration.canceled += instance.OnAcceleration;
                @AngVelocity.started += instance.OnAngVelocity;
                @AngVelocity.performed += instance.OnAngVelocity;
                @AngVelocity.canceled += instance.OnAngVelocity;
                @PhonePosition.started += instance.OnPhonePosition;
                @PhonePosition.performed += instance.OnPhonePosition;
                @PhonePosition.canceled += instance.OnPhonePosition;
                @PhoneRotation.started += instance.OnPhoneRotation;
                @PhoneRotation.performed += instance.OnPhoneRotation;
                @PhoneRotation.canceled += instance.OnPhoneRotation;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAttitude(InputAction.CallbackContext context);
        void OnLinearAcceleration(InputAction.CallbackContext context);
        void OnAcceleration(InputAction.CallbackContext context);
        void OnAngVelocity(InputAction.CallbackContext context);
        void OnPhonePosition(InputAction.CallbackContext context);
        void OnPhoneRotation(InputAction.CallbackContext context);
    }
}