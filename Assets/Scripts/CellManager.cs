using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Panel))]
public class CellManager : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Settings settings;
    [SerializeField] private int levelsCount = 3;
    [SerializeField] private int cellsOnNewLvl = 3;
    
    internal List<GameObject> AllCellsList = new List<GameObject>();
    internal readonly List<string> IgnoreCells = new List<string>();

    private readonly System.Random _rng = new System.Random();
    private Panel _panel;
    private int _level;

    public GameObject RightCell { get; private set; }
    private void Start()
    {
        Setup();
        _panel = GetComponent<Panel>();
    }

    private void Setup()
    {
        NewLevel();
    }

    public void NewLevel()
    {
        if (_level >= levelsCount)
        {
            _panel.DisableButtons(AllCellsList);
            return;
        }

        GenerateLevel();
        IgnoreCells.Add(SetupRightCell());
        _level++;
    }

    public void ResetLevel() => _level = 0;

    private GameSet ChooseSet()
    {
        var gameSets = settings.gameSets;
        if (gameSets == null) throw new ArgumentNullException(nameof(AllCellsList));

        var availableSets = gameSets
            .Where(x => x.images.Length >= AllCellsList.Count + cellsOnNewLvl)
            .ToList();
        if (availableSets.Count == 0) throw new ArgumentException("Not enough images in sets");

        var index = _rng.Next(availableSets.Count);
        return gameSets[index];
    }

    private void GenerateLevel()
    {
        var currentGameSet = ChooseSet();

        var spriteToChoose = currentGameSet
            .images
            .Where(x => !IgnoreCells.Contains(x.name))
            .ToList();
        foreach (var oldCell in AllCellsList)
        {
            var sprite = spriteToChoose[_rng.Next(spriteToChoose.Count)];
            RenewCell(oldCell, sprite);
            spriteToChoose.Remove(sprite);
        }

        GenerateNewCells(spriteToChoose);
    }
    
    private static void RenewCell(GameObject oldCell, Sprite newData)
    {
        oldCell.GetComponentsInChildren<Image>()
            .Last()
            .sprite = newData;
        oldCell.name = newData.name;
    }
    private void GenerateNewCells(IList<Sprite> cellsToChoose )
    {
        var firstly = AllCellsList.Count == 0;
        for (var i = 0; i < cellsOnNewLvl; i++)
        {
            var newCell = Instantiate(cellPrefab, transform, false);
            var sprite = cellsToChoose[_rng.Next(cellsToChoose.Count-1)];
            cellsToChoose.Remove(sprite);

            newCell
                .GetComponentsInChildren<Image>()
                .Last()
                .sprite = sprite;
            newCell.name = sprite.name;
            newCell.SetActive(true);
            AllCellsList.Add(newCell);
        }

        if (firstly)
            AllCellsList.ForEach(x =>
            {
                const float shakeTime = 3f;
                x
                    .transform
                    .DOPunchScale(new Vector3(-1f, -1f, 0f), shakeTime, 3, 0);
            });
        else DOTween.PlayingTweens()?.ForEach(x => x.Complete());
    }

    private string SetupRightCell()
    {
        var cell = AllCellsList[_rng.Next(AllCellsList.Count - 1)];
        settings.textField.Text.text = $"Find {cell.name}";
        RightCell = cell;
        return cell.name;
    }
}