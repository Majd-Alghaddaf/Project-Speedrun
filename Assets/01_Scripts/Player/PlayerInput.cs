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
        if(Keyboard.current.cKey.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            SetGamepadAsInputDevice();
        }
        else if(Gamepad.current.selectButton.wasPressedThisFrame)
        {
            SetKeyboardAndMouseAsInputDevice();
        }
    }

    private void SetKeyboardAndMouseAsInputDevice()
    {
        Debug.LogWarning("Keyboard & Mouse Enabled - Gamepad Disabled");

        InputSystem.EnableDevice(Keyboard.current);
        InputSystem.EnableDevice(Mouse.current);
        InputSystem.DisableDevice(Gamepad.current);

        KeyboardAndMouseEnabled = true;
    }

    private void SetGamepadAsInputDevice()
    {
        Debug.LogWarning("Keyboard & Mouse Disabled - Gamepad Enabled");

        InputSystem.DisableDevice(Keyboard.current);
        InputSystem.DisableDevice(Mouse.current);
        InputSystem.EnableDevice(Gamepad.current);

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
