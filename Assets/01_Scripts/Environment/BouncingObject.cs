using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Config.Instance.playerTag))
        {
            PlayerBouncing playerBouncing = collision.gameObject.GetComponent<PlayerBouncing>();
            playerBouncing.OnTouchingBouncingObject();

            gameObject.SetActive(false);
        }
    }
}
