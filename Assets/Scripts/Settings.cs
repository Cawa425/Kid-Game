using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Settings : MonoBehaviour
{
     public static Settings Instance;
     public Sprite[] numberImages;
     public Sprite[] digitalImages;

     
     public GameObject blackScreen;
     public const float FadeTime = 3f;

     internal static Tween FadingTween;
     private void Awake()
     {
          Instance = this;
     }

     private void Start()
     { 
          DOTween.Init();
     }

     public void FadeOut()
     {
          if (FadingTween!=null && FadingTween.IsPlaying()) return;
          FadingTween = blackScreen.GetComponent<Image>().DOFade(0, FadeTime);
     }
     public void FadeIn()
     {
         if (FadingTween!=null && FadingTween.IsPlaying()) return;
          FadingTween = blackScreen.GetComponent<Image>().DOFade(1, FadeTime);
     }
}
