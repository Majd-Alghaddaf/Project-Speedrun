using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    [SerializeField] float secondsBeforeEndingGame = 2f;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] ParticleSystem gameEndParticleSystem;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Config.Instance.playerTag) == false) { return; }

        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        if (!playerMovement) { return; }

        playerMovement.SetActionsLocked(true);
        Timer.Instance.Pause();
        SaveScore();

        musicAudioSource.Stop();
        _audioSource.Play();

        gameEndParticleSystem.Play();

        StartCoroutine(ReturnToHomepage());
    }

    private void SaveScore()
    {
        string currentTime = Timer.Instance.GetTimeString();

        if (!PlayerPrefs.HasKey("bestTime"))
        {
            PlayerPrefs.SetString("bestTime", currentTime);
            PlayerPrefs.Save();
            return;
        }

        if (Timer.Instance.GetTimeFloat() > PlayerPrefs.GetFloat("bestTime"))
        {
            PlayerPrefs.SetString("bestTime", currentTime);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator ReturnToHomepage()
    {
        yield return new WaitForSeconds(secondsBeforeEndingGame);
        SceneController.Instance.HomePage();
    }
}
