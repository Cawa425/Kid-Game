using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResetGameFader:MonoBehaviour
{
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeTime = 3f;
    internal Tween _fadingTween;
    public void FadeOut()
    {
        Fade(0);

    }
    public void FadeIn()
    {
         Fade(1);
    }

    private void Fade(float value)
    {
        if (_fadingTween!=null && _fadingTween.IsPlaying()) return;
        _fadingTween = blackScreen.DOFade(value, fadeTime);
    }
}