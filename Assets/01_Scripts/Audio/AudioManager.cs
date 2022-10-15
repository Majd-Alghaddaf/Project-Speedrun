using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public void PlayAudioClipOneShot(AudioSource audioSource, AudioClip audioClip, float volumeScale = 1f)
    {
        audioSource.volume = volumeScale;
        audioSource.PlayOneShot(audioClip, volumeScale);
    }

    public void PlayAudioClipInAudioSource(AudioSource audioSource, AudioClip audioClip, float volumeScale = 1f)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volumeScale;
        audioSource.Play();
    }

    public void StopAudioFromSource(AudioSource audioSource)
    {
        audioSource.Stop();
    }
}
