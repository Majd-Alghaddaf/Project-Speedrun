using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private string groundLayerMask = "Ground";

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == groundLayerMask)
        {
            OnGroundEnter();
        }
    }

    private void OnGroundEnter()
    {
        _playerMovement.SetIsGrounded(true);
        _playerMovement.SetCanJump(true);
        _playerMovement.SetCanDoubleJump(true);

        _playerMovement.SetCanLongJump(true);
        _playerMovement.InterruptLongJumpLock();

        StartCoroutine(_playerMovement.EnableWasRecentlyGroundedForDuration());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == groundLayerMask)
        {
            OnGroundExit();
        }
    }

    private void OnGroundExit()
    {
        _playerMovement.SetIsGrounded(false);
        StartCoroutine(_playerMovement.DisableFirstJumpAfterSeconds());
    }
}
