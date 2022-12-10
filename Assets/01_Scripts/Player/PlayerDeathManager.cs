using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField] float fadeInDuration = 1f;
    [SerializeField] float fadeOutDuration = 1f;

    private PlayerMovement _playerMovement;
    private GroundChecker _groundChecker;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _groundChecker = GetComponentInChildren<GroundChecker>();
    }

    public IEnumerator OnDeath()
    {
        _playerMovement.SetActionsLocked(true);
        _playerMovement.PlayDeathAnimation();
        _groundChecker.gameObject.SetActive(false);
        Timer.Instance.Stop();

        yield return FadeOut();

        PositionResetter.Instance.ResetPlayerPosition();

        yield return FadeIn();

        _groundChecker.gameObject.SetActive(true);
        _playerMovement.SetActionsLocked(false);
        Timer.Instance.Continue();
    }

    private IEnumerator FadeOut()
    {
        //todo
        yield return new WaitForSeconds(fadeOutDuration);
    }

    private IEnumerator FadeIn()
    {
        //todo
        yield return new WaitForSeconds(fadeInDuration);
    }
}
