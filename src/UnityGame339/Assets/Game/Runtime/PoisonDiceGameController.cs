using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class PoisonDiceGameController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultsPanel;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI poisonDiceText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI gameplayHighScoreText;
        [SerializeField] private TextMeshProUGUI resultsHighScoreText;
        [SerializeField] private TextMeshProUGUI lastRollText;
        [SerializeField] private TextMeshProUGUI resultsHeaderText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button rollButton;
        [SerializeField] private Button giveUpButton;
        [SerializeField] private Button restartButton;
        
        [Header("Result Styling")]
        [SerializeField] private Color safeRollColor = new Color(0.16f, 0.75f, 0.24f);
        [SerializeField] private Color poisonRollColor = new Color(0.86f, 0.18f, 0.18f);
        [SerializeField] private Color resultsWinColor = new Color(0.98f, 0.84f, 0.26f);
        [SerializeField] private Color resultsLoseColor = new Color(0.9f, 0.15f, 0.2f);

        [Header("Diagnostics")]
        [SerializeField] private bool enableDebugLogs = true;

        private IGameLogger _logger = NullGameLogger.Instance;
        private bool _hasInjectedLogger;
        private PoisonDiceGameModel _model;
        private PoisonDiceScreenViewModel _viewModel;
        private PoisonDiceHudView _hudView;
        private PoisonDiceResultsView _resultsView;
        private PoisonDiceAudioView _audioView;

        private void Awake()
        {
            if (!_hasInjectedLogger)
            {
                _logger = enableDebugLogs
                    ? new UnityDebugGameLogger("PoisonDice", this)
                    : NullGameLogger.Instance;
            }

            _hudView = GetComponent<PoisonDiceHudView>();
            _resultsView = GetComponent<PoisonDiceResultsView>();
            _audioView = GetComponent<PoisonDiceAudioView>();
            _model = new PoisonDiceGameModel(RollDie, _logger);
            _viewModel = new PoisonDiceScreenViewModel(
                _model,
                safeRollColor,
                poisonRollColor,
                resultsWinColor,
                resultsLoseColor);

            _hudView?.Initialize(
                titlePanel,
                gameplayPanel,
                resultsPanel,
                poisonDiceText,
                scoreText,
                gameplayHighScoreText,
                resultsHighScoreText,
                lastRollText,
                statusText,
                resultsHeaderText,
                finalScoreText,
                statusText); // does this need to be smth else?

            _resultsView?.Initialize(resultsHeaderText, finalScoreText, statusText);
        }

        private void Start()
        {
            startButton?.onClick.AddListener(StartNewRound);
            rollButton?.onClick.AddListener(RollDice);
            giveUpButton?.onClick.AddListener(GiveUp);
            restartButton?.onClick.AddListener(RestartRound);

            if (_viewModel != null)
            {
                _viewModel.Changed += RefreshView;
            }

            _logger.Log("MVVM composition root started. Waiting on the title screen.");
            _viewModel?.Initialize();
        }

        private void OnDestroy()
        {
            startButton?.onClick.RemoveListener(StartNewRound);
            rollButton?.onClick.RemoveListener(RollDice);
            giveUpButton?.onClick.RemoveListener(GiveUp);
            restartButton?.onClick.RemoveListener(RestartRound);

            if (_viewModel != null)
            {
                _viewModel.Changed -= RefreshView;
            }
        }

        public void SetLogger(IGameLogger logger)
        {
            _logger = logger ?? NullGameLogger.Instance;
            _hasInjectedLogger = logger != null;
            _model?.SetLogger(_logger);
        }

        private void StartNewRound()
        {
            _viewModel?.StartGame();
        }

        private void RollDice()
        {
            
            _viewModel?.Roll();
        }

        private void GiveUp()
        {
            _viewModel?.GiveUp();
        }

        private void RestartRound()
        {
            _viewModel?.Restart();
        }

        private int RollDie()
        {
            return Random.Range(1, 7);
        }

        private void RefreshView()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (_hudView != null)
            {
                _hudView.Render(_viewModel);
            }
            else
            {
                RenderLegacyView();
            }

            _resultsView?.Render(_viewModel);
            ApplyButtonState();
        }

        private void ApplyButtonState()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (startButton != null) startButton.interactable = _viewModel.CanStart;
            if (rollButton != null) rollButton.interactable = _viewModel.CanRoll;
            if (giveUpButton != null) giveUpButton.interactable = _viewModel.CanGiveUp;
            if (restartButton != null) restartButton.interactable = _viewModel.CanRestart;
        }

        private void RenderLegacyView()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (titlePanel != null) titlePanel.SetActive(_viewModel.ShowTitle);
            if (gameplayPanel != null) gameplayPanel.SetActive(_viewModel.ShowGameplay);
            if (resultsPanel != null) resultsPanel.SetActive(_viewModel.ShowResults);

            SetText(poisonDiceText, _viewModel.PoisonLabel);
            SetText(scoreText, _viewModel.ScoreLabel);
            SetText(gameplayHighScoreText, _viewModel.HighScoreLabel);
            SetText(resultsHighScoreText, _viewModel.HighScoreLabel);
            SetText(lastRollText, _viewModel.LastRollLabel);
            SetText(resultsHeaderText, _viewModel.ResultsHeader);
            SetText(finalScoreText, _viewModel.FinalScoreLabel);
            SetText(statusText, _viewModel.StatusLabel);
            SetTextColor(lastRollText, _viewModel.LastRollColor);
            SetTextColor(statusText, _viewModel.StatusColor);
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetTextColor(TMP_Text text, Color color)
        {
            if (text != null)
            {
                text.color = color;
            }
        }
    }
}
