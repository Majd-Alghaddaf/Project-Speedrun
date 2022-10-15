using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private string groundLayerMask = "Ground";

    private PlayerMovement _playerMovement;

    private void Start()
    {
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == groundLayerMask)
        {
            _playerMovement.SetIsGrounded(true);
            _playerMovement.SetCanDoubleJump(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == groundLayerMask)
        {
            _playerMovement.SetIsGrounded(false);
        }
    }
}
