using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(ResetGameFader))]
public class Settings : MonoBehaviour
{
     public ResetGameFader resetGameFader;
     public List<GameSet> gameSets;
     public UIFind textField;
     private void Start()
     { 
          DOTween.Init();
     }
}
