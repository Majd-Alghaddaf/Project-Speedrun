using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    [SerializeField] float fadeOutTime = 1f;
    [SerializeField] float fadeInTime = 1f;

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeOut()
    {
        _canvasGroup.DOFade(1, fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
    }

    public IEnumerator FadeIn()
    {
        _canvasGroup.DOFade(0, fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
    }
}
