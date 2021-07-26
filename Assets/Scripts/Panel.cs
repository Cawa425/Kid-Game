using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    [SerializeField] private Button resetButton;
    private CellManager _cm;
    private Settings _settings;
    private Image _mainPanel;

    
    public void Start()
    {
        _cm = GetComponent<CellManager>();
        _settings = FindObjectOfType<Settings>();
        _mainPanel = GetComponent<Image>();
    }

    public void OnClick(Button button)
    {
        if (button.name == _cm.RightCell.name) CorrectTap(button);
        else WrongTap(button);
    }

    private static void WrongTap(Component button)
    {
        button.gameObject
            .transform
            .DOPunchPosition(Vector3.right * 2f, 2f);
    }
    
    private void CorrectTap(Component button)
    {
        button.GetComponent<ParticleSystem>().Play(); 
        _cm.NewLevel();
    }

    public void DisableButtons(List<GameObject> allCellsList)
    {
        //disable buttons interact
        allCellsList.ForEach(x => x
            .GetComponentsInChildren<Image>()
            .ToList()
            .ForEach(z => z.raycastTarget = false));
        resetButton.gameObject.SetActive(true);
    }

    //button Event
    public void ResetButtonClick()
    {
        resetButton.interactable = false;
        ResetGame();
    }
    
    private void ResetGame()
    {
        _settings.resetGameFader.FadeIn();
        resetButton.GetComponent<Button>().interactable = false;
        _settings.resetGameFader._fadingTween.OnComplete(() =>
        {
            ClearValues();
            _settings.resetGameFader.FadeOut();
            _settings.resetGameFader._fadingTween.OnComplete(_cm.NewLevel);
        });

    }
    private void ClearValues()
    {
        _cm.AllCellsList.ForEach(Destroy);
        _cm.AllCellsList.Clear();
        _cm.ResetLevel();
        _cm.IgnoreCells.Clear();
        _settings.textField.Text.text="";
        resetButton.interactable = true;
        resetButton.gameObject.SetActive(false);
        _mainPanel.raycastTarget = true;
    }
}