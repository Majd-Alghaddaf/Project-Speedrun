using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(Config.Instance.playerTag))
        {
            PlayerBouncing playerBouncing = collision.collider.GetComponent<PlayerBouncing>();
            playerBouncing.OnTouchingBouncingObject();
        }
    }
}
