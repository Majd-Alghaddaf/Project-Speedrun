using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    private AudioSource _mainAudioSource;

    private void Awake()
    {
        _mainAudioSource = GetComponent<AudioSource>();
    }
    public void PlayAudioClipOneShotFromMainSource(AudioClip audioClip, float volumeScale)
    {
        AudioManager.Instance.PlayAudioClipOneShot(_mainAudioSource, audioClip, volumeScale);
    }

    public void PlayOneShotRandomAudioClipFromArrayFromMainSource(AudioClip[] audioClip, float volumeScale)
    {
        if (audioClip.Length <= 0)
        {
            Debug.LogError("Forgot to add audio clips!");
            return;
        }

        int randomAudioClipIndex = Random.Range(0, audioClip.Length);
        AudioClip randomAudioClip = audioClip[randomAudioClipIndex];

        AudioManager.Instance.PlayAudioClipOneShot(_mainAudioSource, randomAudioClip, volumeScale);
    }

    public void PlayOneShotRandomAudioClipFromArray(AudioSource audioSource, AudioClip[] audioClip, float volumeScale)
    {
        if (audioClip.Length <= 0)
        {
            Debug.LogError("Forgot to add audio clips!");
            return;
        }

        int randomAudioClipIndex = Random.Range(0, audioClip.Length);
        AudioClip randomAudioClip = audioClip[randomAudioClipIndex];

        AudioManager.Instance.PlayAudioClipOneShot(audioSource, randomAudioClip, volumeScale);
    }

    public void PlayAudioClipOneShot(AudioSource audioSource, AudioClip audioClip, float volumeScale)
    {
        AudioManager.Instance.PlayAudioClipOneShot(audioSource, audioClip, volumeScale);
    }

    public void PlayAudioClip(AudioSource audioSource, AudioClip audioClip, float volumeScale)
    {
        AudioManager.Instance.PlayAudioClipInAudioSource(audioSource, audioClip, volumeScale);
    }
    public void StopAudioSource(AudioSource audioSource)
    {
        AudioManager.Instance.StopAudioFromSource(audioSource);
    }
}
