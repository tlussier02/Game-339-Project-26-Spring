using TMPro;
using UnityEngine;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchGameController : MonoBehaviour
    {
        [Header("Rules")]
        [SerializeField] private int gridSize = 9;
        [SerializeField] private int minimumMatchCount = 3;
        [SerializeField] private int baseMatchScore = 100;
        [SerializeField] private int extraCropMultiplierStep = 1;
        [SerializeField] private float roundDurationSeconds = 180f;
        [SerializeField] private int targetScore = 2500;

        [Header("Dependencies")]
        [SerializeField] private MonoBehaviour boardProvider;

        [Header("HUD")]
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text highScoreLabel;
        [SerializeField] private TMP_Text timerLabel;
        [SerializeField] private TMP_Text selectionCountLabel;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private FarmMatchGameOverPanel gameOverPanel;

        private IFarmMatchBoard _board;
        private FarmMatchGameModel _model;
        private FarmMatchScreenViewModel _viewModel;

        private void Awake()
        {
            _board = boardProvider as IFarmMatchBoard;
            if (_board == null)
            {
                Debug.LogWarning("FarmMatchGameController requires a boardProvider that implements IFarmMatchBoard.");
                enabled = false;
                return;
            }

            var rules = new FarmMatchRules
            {
                GridSize = gridSize,
                MinimumMatchCount = minimumMatchCount,
                BaseMatchScore = baseMatchScore,
                ExtraCropMultiplierStep = extraCropMultiplierStep,
                RoundDurationSeconds = roundDurationSeconds,
                TargetScore = targetScore > 0 ? (int?)targetScore : null
            };

            _model = new FarmMatchGameModel(_board, new UnityTimeProvider(), new FarmMatchScoreService(), rules);
            _viewModel = new FarmMatchScreenViewModel(_model);
            _model.StateChanged += Render;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetRestartCallback(RestartRound);
            }

            Render();
        }

        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.StateChanged -= Render;
            }
        }

        private void Update()
        {
            if (_model != null)
            {
                _model.Tick();
            }
        }

        public void StartRound()
        {
            _model?.StartNewRound();
        }

        public void RestartRound()
        {
            _model?.StartNewRound();
        }

        public void SelectCell(int row, int column)
        {
            _model?.TrySelectCell(row, column);
        }

        public void SubmitCurrentSelection()
        {
            if (_model == null)
            {
                return;
            }

            _model.TryResolveSelection(out _, out _);
        }

        public void CancelCurrentSelection()
        {
            _model?.CancelSelection(FarmMatchSelectionClearReason.ClickedOutsideGrid);
        }

        public void StopRound()
        {
            _model?.StopRoundEarly();
        }

        private void Render()
        {
            if (_viewModel == null)
            {
                return;
            }

            SetText(scoreLabel, _viewModel.ScoreLabel);
            SetText(highScoreLabel, _viewModel.HighScoreLabel);
            SetText(timerLabel, _viewModel.TimerLabel);
            SetText(selectionCountLabel, _viewModel.SelectionCountLabel);
            SetText(statusLabel, _viewModel.StatusLabel);

            if (gameOverPanel != null)
            {
                gameOverPanel.Render(_viewModel);
            }
        }

        private static void SetText(TMP_Text label, string value)
        {
            if (label != null)
            {
                label.text = value;
            }
        }
    }
}
