using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Set",menuName = "Game Set")]
public class GameSet : ScriptableObject
{
    [SerializeField] public Sprite[] images;
}