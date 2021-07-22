using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CellPanel : MonoBehaviour
{
    private const int LevelsCount = 3;
    private const int CellsOnNewLvl = 3;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Text findTextField;
    [SerializeField] private GameObject resetButton;

    private readonly List<GameObject> _allCellsList = new List<GameObject>();
    private readonly List<string> _ignoreCells = new List<string>();
    private readonly Random _rng = new Random();
    private int _cycle;
    private string _rightCellName;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        NewLevel();
        var startColor = findTextField.color;
        findTextField.DOColor(new Color(startColor.r, startColor.g, startColor.b, 0), 0.0001f);
        findTextField.DOFade(1, Settings.FadeTime);
    }

    private GameSetType ChooseSet()
    {
        while (true)
        {
            var allSets = GameTypesFactory.GetAllGameSetsName().ToList();
            var index = _rng.Next(allSets.Count);
            var gameSetName = allSets[index];
            var gameSet = GameTypesFactory.GetGameObject(gameSetName);
            //return if game set have enough items
            var images = gameSet.Images.Where(x => !_ignoreCells.Contains(x.name)).ToArray();
            if (images.Length >= _allCellsList.Count + CellsOnNewLvl) return gameSet;
        }
    }

    private void NewLevel()
    {
        if (_cycle >= LevelsCount)
        {
            DisableGameButtons();
            return;
        }

        Settings.FadingTween.Kill();
        GenerateLevel();
        RightButtonSetup();
        _cycle++;
    }

    private void GenerateLevel()
    {
        var currentGameSet = ChooseSet();

        var spriteToChoose = currentGameSet
            .Images
            .Where(x => !_ignoreCells.Contains(x.name))
            .ToList();
        foreach (var oldCell in _allCellsList)
        {
            var sprite = spriteToChoose[_rng.Next(spriteToChoose.Count)];
            RenewCell(oldCell, sprite);
            spriteToChoose.Remove(sprite);
        }

        GenerateNewCells(spriteToChoose);
    }

    private void GenerateNewCells(IList<Sprite> cellsToChoose)
    {
        var shake = _allCellsList.Count == 0;
        for (var i = 0; i < 3; i++)
        {
            var newCell = Instantiate(cellPrefab, transform, true);
            var sprite = cellsToChoose[_rng.Next(cellsToChoose.Count)];
            cellsToChoose.Remove(sprite);

            newCell
                .GetComponentsInChildren<Image>()
                .Last()
                .sprite = sprite;
            newCell.name = sprite.name;
            newCell.SetActive(true);


            _allCellsList.Add(newCell);
        }

        if (shake)
            _allCellsList.ForEach(x =>
            {
                const float shakeTime = 3f;
                x
                    .transform
                    .DOPunchScale(new Vector3(-1f, -1f, 0f), shakeTime, 3, 0);
            });
        else
            DOTween.PlayingTweens()?.ForEach(x => x.Complete());
    }

    private static void RenewCell(GameObject oldCell, Sprite newData)
    {
        oldCell
            .GetComponentsInChildren<Image>()
            .Last()
            .sprite = newData;
        oldCell.name = newData.name;
    }

    private void RightButtonSetup()
    {
        var cellName = _allCellsList[_rng.Next(_allCellsList.Count - 1)].name;
        _rightCellName = cellName;
        //exclude for next levels
        findTextField.text = $"Find {_rightCellName}";
        _ignoreCells.Add(cellName);
    }

    public void OnClick(Button button)
    {
        if (button.name == _rightCellName) CorrectTap(button);
        else WrongTap(button);
    }

    private static void WrongTap(Component button)
    {
        const float punchTime = 2f;
        button
            .gameObject
            .transform
            .DOPunchPosition(
                Vector3.right * 2f,
                punchTime);
    }

    private void CorrectTap(Component button)
    {
        button.GetComponent<ParticleSystem>().Play();
        NewLevel();
    }

    private void DisableGameButtons()
    {
        //disable buttons interact
        _allCellsList.ForEach(x => x.GetComponentsInChildren<Image>().ToList().ForEach(z => z.raycastTarget = false));
        resetButton.SetActive(true);
    }

    //button Event
    public void ResetButtonClick()
    {
        Settings.Instance.FadeIn();
        resetButton.GetComponent<Button>().interactable = false;
        Settings.FadingTween.OnComplete(() =>
        {
            _allCellsList.ForEach(Destroy);
            _allCellsList.Clear();
            _cycle = 0;
            _ignoreCells.Clear();
            findTextField.text = "";
            resetButton.GetComponent<Button>().interactable = true;
            resetButton.SetActive(false);
            transform.GetComponent<Image>().raycastTarget = true;
            Settings.Instance.FadeOut();
            Settings.FadingTween.OnComplete(NewLevel);
        });
    }
}