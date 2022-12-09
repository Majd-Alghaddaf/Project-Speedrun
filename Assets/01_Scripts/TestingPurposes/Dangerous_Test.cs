using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dangerous_Test : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag))
        {
            Debug.Log('l');
            PositionResetter.Instance.ResetPlayerPosition();
        }
    }
}
