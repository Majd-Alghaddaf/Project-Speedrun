using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SlowMoObject : MonoBehaviour
{
    private CircleCollider2D _slowMoCollider;
    private bool _canBeUsedToLaunch = true;

    private void Awake()
    {
        _slowMoCollider = GetComponent<CircleCollider2D>();

        if(_slowMoCollider.isTrigger == false)
        {
            Debug.LogError("Collider is supposed to be trigger!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag))
        {
            PlayerSlowMoLaunch playerSlowMoComponent = collision.GetComponent<PlayerSlowMoLaunch>();
            if(playerSlowMoComponent.SetNearbySlowMoObject(this))
            {
                playerSlowMoComponent.canSlowMo = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag))
        {
            collision.GetComponent<PlayerSlowMoLaunch>().canSlowMo = false;
        }
    }

    public IEnumerator SetChargeCooldown(float launchChargeCooldown)
    {
        _canBeUsedToLaunch = false;
        yield return new WaitForSeconds(launchChargeCooldown);
        _canBeUsedToLaunch = true;
    }

    public bool CanBeUsedToLaunch()
    {
        return _canBeUsedToLaunch;
    }
}
