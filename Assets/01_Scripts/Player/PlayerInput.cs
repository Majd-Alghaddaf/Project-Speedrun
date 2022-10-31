using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerActions _playerActions;

    public bool KeyboardAndMouseEnabled = true;

    private void Awake()
    {
        _playerActions = new PlayerActions();

        SetKeyboardAndMouseAsInputDevice();
    }
    private void OnEnable()
    {
        _playerActions.BaseActionMap.Enable();
    }

    private void OnDisable()
    {
        _playerActions.BaseActionMap.Disable();
    }

    private void Update()
    {
        if(Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            SetGamepadAsInputDevice();
        }
        else if(Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
        {
            SetKeyboardAndMouseAsInputDevice();
        }
    }

    private void SetKeyboardAndMouseAsInputDevice()
    {
        if(Keyboard.current == null && Mouse.current == null) { return; }

        Debug.LogWarning("Keyboard & Mouse Enabled - Gamepad Disabled");

        InputSystem.EnableDevice(Keyboard.current);
        InputSystem.EnableDevice(Mouse.current);
        KeyboardAndMouseEnabled = true;

        if (Gamepad.current != null)
            InputSystem.DisableDevice(Gamepad.current);
    }

    private void SetGamepadAsInputDevice()
    {
        if(Gamepad.current == null) { return; }

        Debug.LogWarning("Keyboard & Mouse Disabled - Gamepad Enabled");

        InputSystem.EnableDevice(Gamepad.current);

        if(Keyboard.current != null && Mouse.current != null)
        {
            InputSystem.DisableDevice(Keyboard.current);
            InputSystem.DisableDevice(Mouse.current);
        }
        KeyboardAndMouseEnabled = false;
    }

    public float GetMovementInput()
    {
        return _playerActions.BaseActionMap.Movement.ReadValue<float>();
    }

    public bool GetJumpPressedInput()
    {
        return _playerActions.BaseActionMap.Jump.triggered;
    }   
    
    public bool GetJumpHoldInput()
    {
        return _playerActions.BaseActionMap.HoldingJump.IsPressed();
    }

    public bool GetDashInput()
    {
        return _playerActions.BaseActionMap.Dash.triggered;
    }

    public bool GetSlowMoLaunchHoldInput()
    {
        return _playerActions.BaseActionMap.HoldingSlowMoLaunch.IsPressed();
    }

    public Vector2 GetMousePosition()
    {
        return _playerActions.BaseActionMap.MousePosition.ReadValue<Vector2>();
    }

    public Vector2 GetLeftJoystickDirection()
    {
        return _playerActions.BaseActionMap.LeftJoystickDirection.ReadValue<Vector2>();
    }    
}
