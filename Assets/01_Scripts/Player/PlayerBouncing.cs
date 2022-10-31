using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBouncing : MonoBehaviour
{
    [SerializeField] float verticalBouncingForceValue;
    [SerializeField] float timeBeforeAllowingDoubleJump = 0.2f;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnTouchingBouncingObject()
    {
        Vector2 bounceForceVector = new Vector2(0f, verticalBouncingForceValue);
        _playerMovement.Bounce(bounceForceVector, timeBeforeAllowingDoubleJump);
    }
}
