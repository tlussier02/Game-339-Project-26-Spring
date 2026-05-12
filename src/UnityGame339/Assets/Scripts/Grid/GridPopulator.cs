using System.Collections.Generic;
using Game;
using Game.Runtime.FarmMatch;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridPopulator : MonoBehaviour, IFarmMatchBoard
{
    private const string HighScorePrefsKey = "FarmMatch.HighScore";

    [Header("Board")]
    public Vector3Int gridStartCell = new  Vector3Int(0, 5, 0);
    public int gridSize = 9;
    public Tilemap Tilemap;
    public List<TileBase> TilesToPlace;
    [SerializeField] private Color defaultTileColor = Color.white;
    [SerializeField] private Color selectedTileColor = new Color(1f, 0.92f, 0.45f, 1f);

    [Header("Rules")]
    [SerializeField] private int minimumMatchCount = 3;
    [SerializeField] private int baseMatchScore = 100;
    [SerializeField] private int extraCropMultiplierStep = 1;
    [SerializeField] private float roundDurationSeconds = 180f;
    [SerializeField] private int targetScore = 2500;
    [SerializeField] private int targetScoreIncreasePerRound = 500;

    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text goalText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text startButtonLabel;
    [SerializeField] private FarmMatchGameOverPanel resultsPanel;
    [SerializeField] private bool loadResultsSceneOnRoundEnd;
    [SerializeField] private string resultsSceneName = "LiTestingScene";
    [SerializeField] private string restartSceneName = "FM_GameScreen";

    private readonly Dictionary<TileBase, FarmCropType> _tileToCropType = new Dictionary<TileBase, FarmCropType>();
    private readonly HashSet<GridPosition> _highlightedPositions = new HashSet<GridPosition>();
    private FarmMatchGameModel _model;
    private FarmMatchScreenViewModel _viewModel;
    private Camera _inputCamera;

    private void Awake()
    {
        ResolveSceneReferences();
        BuildTileLookup();

        var rules = new FarmMatchRules
        {
            GridSize = gridSize,
            MinimumMatchCount = minimumMatchCount,
            BaseMatchScore = baseMatchScore,
            ExtraCropMultiplierStep = extraCropMultiplierStep,
            RoundDurationSeconds = roundDurationSeconds,
            TargetScore = targetScore > 0 ? (int?)targetScore : null,
            TargetScoreIncreasePerRound = targetScoreIncreasePerRound
        };

        _model = new FarmMatchGameModel(this, new UnityTimeProvider(), new FarmMatchScoreService(), rules);
        _model.SetHighScore(PlayerPrefs.GetInt(HighScorePrefsKey, 0));
        _model.HighScoreChanged += SaveHighScore;
        _viewModel = new FarmMatchScreenViewModel(_model);
        _viewModel.ViewChanged += Render;
        _viewModel.RoundEnded += HandleRoundEnded;

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(HandlePrimaryButtonClick);
            startButton.onClick.AddListener(HandlePrimaryButtonClick);
        }

        if (resultsPanel != null)
        {
            resultsPanel.SetRestartCallback(RestartRound);
        }

        ResetBoard();
        Render();
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.HighScoreChanged -= SaveHighScore;
            _viewModel.ViewChanged -= Render;
            _viewModel.RoundEnded -= HandleRoundEnded;
            _viewModel.Dispose();
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(HandlePrimaryButtonClick);
        }
    }

    private void Update()
    {
        if (_model == null)
        {
            return;
        }

        _model.Tick();
        HandlePointerInput();
        HandleKeyboardInput();
    }

    public bool TryGetCrop(GridPosition position, out FarmCropType cropType)
    {
        if (!IsWithinBounds(position))
        {
            cropType = FarmCropType.None;
            return false;
        }

        var tile = Tilemap != null ? Tilemap.GetTile(GetCellPosition(position)) : null;
        cropType = GetCropType(tile);
        return true;
    }

    public void ReplaceMatchedCrops(IReadOnlyList<GridPosition> matchedPositions)
    {
        if (Tilemap == null || matchedPositions == null)
        {
            return;
        }

        for (var i = 0; i < matchedPositions.Count; i++)
        {
            var cellPosition = GetCellPosition(matchedPositions[i]);
            var currentTile = Tilemap.GetTile(cellPosition);
            Tilemap.SetTile(cellPosition, GetRandomReplacementTile(currentTile));
            SetTileColor(cellPosition, defaultTileColor);
        }
    }

    public void ResetBoard()
    {
        if (Tilemap == null || TilesToPlace == null || TilesToPlace.Count == 0)
        {
            return;
        }

        for (var row = 0; row < gridSize; row++)
        {
            for (var column = 0; column < gridSize; column++)
            {
                Tilemap.SetTile(
                    GetCellPosition(new GridPosition(row, column)),
                    GetRandomTile());
                SetTileColor(GetCellPosition(new GridPosition(row, column)), defaultTileColor);
            }
        }

        _highlightedPositions.Clear();
    }

    private void HandlePrimaryButtonClick()
    {
        if (_model == null)
        {
            return;
        }

        if (_model.State.RoundState == FarmMatchRoundState.Playing)
        {
            if (_model.State.SelectionCount >= minimumMatchCount)
            {
                _model.TryResolveSelection(out _, out _);
                return;
            }

            if (_model.State.SelectionCount > 0)
            {
                _model.CancelSelection(FarmMatchSelectionClearReason.ClickedOutsideGrid);
                return;
            }

            _model.StopRoundEarly();
            return;
        }

        _model.StartNewRound();
    }

    private void RestartRound()
    {
        _model?.StartNewRound();
    }

    private void HandleRoundEnded(FarmMatchRoundResult result)
    {
        if (!loadResultsSceneOnRoundEnd || string.IsNullOrWhiteSpace(resultsSceneName))
        {
            return;
        }

        FarmMatchResultsSession.Set(result, restartSceneName);
        SceneManager.LoadScene(resultsSceneName);
    }

    private static void SaveHighScore(int highScore)
    {
        PlayerPrefs.SetInt(HighScorePrefsKey, highScore);
        PlayerPrefs.Save();
    }

    private void HandlePointerInput()
    {
        if (_model.State.RoundState != FarmMatchRoundState.Playing || !Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (IsPointerOverStartButton())
        {
            return;
        }

        if (TryGetPointerGridPosition(out var position))
        {
            _model.TrySelect(position);
            return;
        }

        _model.CancelSelection(FarmMatchSelectionClearReason.ClickedOutsideGrid);
    }

    private void HandleKeyboardInput()
    {
        if (_model.State.RoundState != FarmMatchRoundState.Playing)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            _model.TryResolveSelection(out _, out _);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _model.CancelSelection(FarmMatchSelectionClearReason.ClickedOutsideGrid);
        }
    }

    private bool IsPointerOverStartButton()
    {
        if (startButton == null)
        {
            return false;
        }

        var buttonTransform = startButton.transform as RectTransform;
        return buttonTransform != null
            && RectTransformUtility.RectangleContainsScreenPoint(buttonTransform, Input.mousePosition, null);
    }

    private bool TryGetPointerGridPosition(out GridPosition position)
    {
        position = default;

        if (_inputCamera == null || Tilemap == null)
        {
            return false;
        }

        var pointerPosition = Input.mousePosition;
        pointerPosition.z = Mathf.Abs(_inputCamera.transform.position.z - Tilemap.transform.position.z);

        var worldPosition = _inputCamera.ScreenToWorldPoint(pointerPosition);
        var cellPosition = Tilemap.WorldToCell(worldPosition);

        if (!TryGetGridPosition(cellPosition, out position))
        {
            return false;
        }

        return Tilemap.GetTile(cellPosition) != null;
    }

    private bool TryGetGridPosition(Vector3Int cellPosition, out GridPosition position)
    {
        var column = cellPosition.x - gridStartCell.x;
        var row = gridStartCell.y - cellPosition.y;

        if (row < 0 || row >= gridSize || column < 0 || column >= gridSize)
        {
            position = default;
            return false;
        }

        position = new GridPosition(row, column);
        return true;
    }

    private Vector3Int GetCellPosition(GridPosition position)
    {
        return gridStartCell + new Vector3Int(position.Column, -position.Row, 0);
    }

    private bool IsWithinBounds(GridPosition position)
    {
        return position.Row >= 0
            && position.Row < gridSize
            && position.Column >= 0
            && position.Column < gridSize;
    }

    private TileBase GetRandomTile()
    {
        return TilesToPlace[Random.Range(0, TilesToPlace.Count)];
    }

    private TileBase GetRandomReplacementTile(TileBase currentTile)
    {
        if (TilesToPlace == null || TilesToPlace.Count == 0)
        {
            return null;
        }

        if (TilesToPlace.Count == 1)
        {
            return TilesToPlace[0];
        }

        TileBase replacementTile;
        do
        {
            replacementTile = GetRandomTile();
        }
        while (replacementTile == currentTile);

        return replacementTile;
    }

    private FarmCropType GetCropType(TileBase tile)
    {
        if (tile == null)
        {
            return FarmCropType.None;
        }

        if (_tileToCropType.TryGetValue(tile, out var cropType))
        {
            return cropType;
        }

        return InferCropType(tile, _tileToCropType.Count);
    }

    private void BuildTileLookup()
    {
        _tileToCropType.Clear();

        if (TilesToPlace == null)
        {
            return;
        }

        for (var i = 0; i < TilesToPlace.Count; i++)
        {
            var tile = TilesToPlace[i];
            if (tile == null)
            {
                continue;
            }

            _tileToCropType[tile] = InferCropType(tile, i);
        }
    }

    private static FarmCropType InferCropType(TileBase tile, int fallbackIndex)
    {
        if (tile != null)
        {
            var tileName = tile.name.ToLowerInvariant();
            if (tileName.Contains("apple"))
            {
                return FarmCropType.Apple;
            }

            if (tileName.Contains("grape"))
            {
                return FarmCropType.Grape;
            }

            if (tileName.Contains("cherry"))
            {
                return FarmCropType.Cherry;
            }

            if (tileName.Contains("kiwi"))
            {
                return FarmCropType.Kiwi;
            }

            if (tileName.Contains("orange"))
            {
                return FarmCropType.Orange;
            }

            if (tileName.Contains("watermelon"))
            {
                return FarmCropType.Watermelon;
            }
        }

        switch (fallbackIndex % 6)
        {
            case 0:
                return FarmCropType.Apple;
            case 1:
                return FarmCropType.Grape;
            case 2:
                return FarmCropType.Cherry;
            case 3:
                return FarmCropType.Kiwi;
            case 4:
                return FarmCropType.Orange;
            default:
                return FarmCropType.Watermelon;
        }
    }

    private void ResolveSceneReferences()
    {
        if (Tilemap == null)
        {
            Tilemap = GetComponentInChildren<Tilemap>();
        }

        if (scoreText == null)
        {
            scoreText = FindText("SCORE - Text ");
        }

        if (goalText == null)
        {
            goalText = FindText("GOAL - Text ");
        }

        if (timerText == null)
        {
            timerText = FindText("Time - Text");
        }

        if (startButton == null)
        {
            startButton = FindButton("Start - Button");
        }

        if (startButtonLabel == null && startButton != null)
        {
            startButtonLabel = startButton.GetComponentInChildren<TMP_Text>(true);
        }

        if (resultsPanel == null)
        {
            resultsPanel = FindFirstObjectByType<FarmMatchGameOverPanel>(FindObjectsInactive.Include);
        }

        _inputCamera = Camera.main;
        if (_inputCamera == null)
        {
            _inputCamera = FindFirstObjectByType<Camera>();
        }
    }

    private void Render()
    {
        if (_model == null || _viewModel == null)
        {
            return;
        }

        UpdateSelectionHighlight();
        SetText(scoreText, _viewModel.ScoreHudLabel);
        SetText(timerText, _viewModel.TimerHudLabel);
        SetText(goalText, _viewModel.GoalHudLabel);

        if (resultsPanel != null)
        {
            resultsPanel.Render(_viewModel);
        }

        if (_viewModel.RoundState == FarmMatchRoundState.Results)
        {
            SetText(startButtonLabel, "RESULTS");
            SetButtonInteractable(resultsPanel == null);
            return;
        }

        if (_viewModel.RoundState == FarmMatchRoundState.Title)
        {
            SetText(startButtonLabel, "START");
            SetButtonInteractable(true);
            return;
        }

        if (_model.State.SelectionCount >= minimumMatchCount)
        {
            SetText(startButtonLabel, "SUBMIT");
            SetButtonInteractable(true);
            return;
        }

        if (_model.State.SelectionCount > 0)
        {
            SetText(startButtonLabel, "CLEAR");
            SetButtonInteractable(true);
            return;
        }

        SetText(startButtonLabel, "END ROUND");
        SetButtonInteractable(true);
    }

    private void SetButtonInteractable(bool isInteractable)
    {
        if (startButton != null)
        {
            startButton.interactable = isInteractable;
        }
    }

    private void UpdateSelectionHighlight()
    {
        if (Tilemap == null || _model == null)
        {
            return;
        }

        var currentSelection = new HashSet<GridPosition>(_model.CurrentSelection);

        foreach (var position in _highlightedPositions)
        {
            if (!currentSelection.Contains(position))
            {
                SetTileColor(GetCellPosition(position), defaultTileColor);
            }
        }

        foreach (var position in currentSelection)
        {
            SetTileColor(GetCellPosition(position), selectedTileColor);
        }

        _highlightedPositions.Clear();
        foreach (var position in currentSelection)
        {
            _highlightedPositions.Add(position);
        }
    }

    private void SetTileColor(Vector3Int cellPosition, Color color)
    {
        if (Tilemap == null)
        {
            return;
        }

        Tilemap.SetTileFlags(cellPosition, TileFlags.None);
        Tilemap.SetColor(cellPosition, color);
    }

    private static TMP_Text FindText(string objectName)
    {
        var target = GameObject.Find(objectName);
        return target != null ? target.GetComponent<TMP_Text>() : null;
    }

    private static Button FindButton(string objectName)
    {
        var target = GameObject.Find(objectName);
        return target != null ? target.GetComponent<Button>() : null;
    }

    private static void SetText(TMP_Text label, string value)
    {
        if (label != null)
        {
            label.text = value;
        }
    }
}
