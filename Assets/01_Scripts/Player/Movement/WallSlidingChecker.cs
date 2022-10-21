using System.Collections;
using UnityEngine;

    public class WallSlidingChecker : MonoBehaviour
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
            _playerMovement.OnWallSlidingEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == groundLayerMask)
        {
            _playerMovement.OnWallSlidingExit();
        }
    }
}