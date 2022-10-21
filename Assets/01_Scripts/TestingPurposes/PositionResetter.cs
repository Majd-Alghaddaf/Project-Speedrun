using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PositionResetter : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] Transform positionResetObject;

    void Update()
    {
        if(Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
        {
            playerObject.transform.position = positionResetObject.position;
        }
    }
}
