using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PositionResetter : MonoBehaviour
{
    #region Singleton
    private static PositionResetter _instance;

    public static PositionResetter Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField] GameObject playerObject;
    [SerializeField] Transform startingPosition;

    [Header("Testing Purposes")]
    [SerializeField] bool multipleCheckpoints = false;
    [SerializeField] List<Transform> checkpoints;

    private Transform resetTransform;

    void Update()
    {
        if(Keyboard.current.leftShiftKey.isPressed && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetPlayerPosition();
        }
        if (Keyboard.current.digit0Key.isPressed)
            resetTransform = startingPosition;
        else if (Keyboard.current.digit1Key.isPressed)
            resetTransform = checkpoints[0];
        else if (Keyboard.current.digit2Key.isPressed)
            resetTransform = checkpoints[1];
        else if (Keyboard.current.digit3Key.isPressed)
            resetTransform = checkpoints[2];
        else if (Keyboard.current.digit4Key.isPressed)
            resetTransform = checkpoints[3];
        else if (Keyboard.current.digit5Key.isPressed)
            resetTransform = checkpoints[4];
    }

    public void ResetPlayerPosition()
    {
        Vector2 resetPosition = Vector2.zero;

        if(multipleCheckpoints)
        {
            if (resetTransform == null)
                resetPosition = startingPosition.position;
            else
                resetPosition = resetTransform.position;
        }
        else
        {
            resetPosition = startingPosition.position;
        }

        playerObject.transform.position = resetPosition;
    }
}
