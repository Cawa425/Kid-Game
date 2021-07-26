using System;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIFind: MonoBehaviour
{
    [SerializeField] private Text text;
    
    public Text Text => text;

    public void Start()
    {
        FadeFindText();
    }

    private void FadeFindText()
    {
        var startColor = Text.color;
        Text.DOColor(new Color(startColor.r, startColor.g, startColor.b, 0), 0.0001f);
        Text.DOFade(1, 3f);
    }
}