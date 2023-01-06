using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag) == false) { return; }

        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        if (!playerMovement) { return; }

        playerMovement.SetActionsLocked(true);
        Timer.Instance.Pause();
    }
}
