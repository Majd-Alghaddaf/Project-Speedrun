using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerDeathManager : MonoBehaviour
{
    [SerializeField] float timeBeforeFadeOut = 0.5f;
    [SerializeField] float timeBetweenFades = 1f;
    [SerializeField] float timeAfterFadeIn = 0.5f;
    [SerializeField] Fader fader;
    [SerializeField] ParticleSystem deathParticleSystem;

    private PlayerMovement _playerMovement;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public IEnumerator OnDeath()
    {
        _playerMovement.SetActionsLocked(true);
        _playerMovement.PlayDeathAnimation();
        _playerMovement.PlayDeathSFX();
        Timer.Instance.Stop();
        deathParticleSystem.Play();

        yield return new WaitForSeconds(timeBeforeFadeOut);
        yield return FadeOut();

        PositionResetter.Instance.ResetPlayerPosition();
        yield return new WaitForSeconds(timeBetweenFades);

        yield return FadeIn();
        yield return new WaitForSeconds(timeAfterFadeIn);

        _playerMovement.SetCanJump(true);
        _playerMovement.SetActionsLocked(false);
        Timer.Instance.Continue();
    }

    private IEnumerator FadeOut()
    {
        yield return fader.FadeOut();
    }

    private IEnumerator FadeIn()
    {
        yield return fader.FadeIn();
    }
}
