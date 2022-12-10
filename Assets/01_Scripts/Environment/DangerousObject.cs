using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag) == false) { return; }

        PlayerDeathManager deathManager = collision.gameObject.GetComponent<PlayerDeathManager>();
        if(!deathManager) { return; }

        StartCoroutine(deathManager.OnDeath());
    }
}
