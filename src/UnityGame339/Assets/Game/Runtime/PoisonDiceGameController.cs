using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class PoisonDiceGameController : MonoBehaviour
    {
        private enum GameState
        {
            Title,
            Playing,
            Results
        }

        [Header("Panels")]
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultsPanel;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI poisonDiceText;
        [SerializeField] private TextMeshProUGUI scoreText;
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

        private GameState _state = GameState.Title;
        private int _poisonDice;
        private int _score;
        private IGameLogger _logger = NullGameLogger.Instance;
        private bool _hasInjectedLogger;

        private void Awake()
        {
            if (!_hasInjectedLogger)
            {
                _logger = enableDebugLogs
                    ? new UnityDebugGameLogger("PoisonDice", this)
                    : NullGameLogger.Instance;
            }
        }

        private void Start()
        {
            startButton?.onClick.AddListener(StartNewRound);
            rollButton?.onClick.AddListener(RollDice);
            giveUpButton?.onClick.AddListener(GiveUp);
            restartButton?.onClick.AddListener(StartNewRound);

            _logger.Log("Controller started. Waiting on the title screen.");
            ShowState(GameState.Title);
        }

        private void OnDestroy()
        {
            startButton?.onClick.RemoveListener(StartNewRound);
            rollButton?.onClick.RemoveListener(RollDice);
            giveUpButton?.onClick.RemoveListener(GiveUp);
            restartButton?.onClick.RemoveListener(StartNewRound);
        }

        public void SetLogger(IGameLogger logger)
        {
            _logger = logger ?? NullGameLogger.Instance;
            _hasInjectedLogger = logger != null;
        }

        private void StartNewRound()
        {
            _poisonDice = RollDie();
            _score = 0;

            SetText(lastRollText, "Roll to begin");
            SetText(statusText, "Avoid this number to keep streaking points");
            SetTextColor(statusText, Color.white);

            UpdateScoreDisplay();
            UpdatePoisonDisplay();
            _logger.Log("Started a new round. Poison number is " + _poisonDice + ". Score reset to 0.");
            ShowState(GameState.Playing);
        }

        private void RollDice()
        {
            if (_state != GameState.Playing)
            {
                _logger.LogWarning("Ignored roll input because the game is in state " + _state + ".");
                return;
            }

            var roll = RollDie();
            if (roll == _poisonDice)
            {
                SetText(lastRollText, $"Rolled {roll} — poisoned!");
                SetTextColor(lastRollText, poisonRollColor);
                _logger.LogWarning("Player rolled " + roll + ", matched the poison number, and lost the round.");
                ShowGameOver(0, $"You hit the poison number ({_poisonDice}).");
                return;
            }

            _score += roll;
            SetText(lastRollText, $"Rolled {roll}");
            SetTextColor(lastRollText, safeRollColor);
            UpdateScoreDisplay();
            SetText(statusText, "Nice roll. Keep going or give up.");
            _logger.Log("Player rolled " + roll + " safely. Score is now " + _score + ".");
        }

        private void GiveUp()
        {
            if (_state != GameState.Playing)
            {
                _logger.LogWarning("Ignored give-up input because the game is in state " + _state + ".");
                return;
            }

            _logger.Log("Player cashed out with " + _score + " points.");
            ShowGameOver(_score, "You chose to cash out.");
        }

        private void ShowGameOver(int finalScore, string message)
        {
            SetText(finalScoreText, $"Final Score: {finalScore}");
            SetText(resultsHeaderText, finalScore == 0
                ? "Bust"
                : "Round Over");
            SetText(statusText, message);
            SetTextColor(statusText, finalScore == 0 ? resultsLoseColor : resultsWinColor);
            _logger.Log("Round finished with final score " + finalScore + ". " + message);
            ShowState(GameState.Results);
        }

        private int RollDie()
        {
            return Random.Range(1, 7);
        }

        private void ShowState(GameState nextState)
        {
            var previousState = _state;
            _state = nextState;

            if (titlePanel != null) titlePanel.SetActive(_state == GameState.Title);
            if (gameplayPanel != null) gameplayPanel.SetActive(_state == GameState.Playing);
            if (resultsPanel != null) resultsPanel.SetActive(_state == GameState.Results);

            if (startButton != null) startButton.interactable = _state == GameState.Title;
            if (rollButton != null) rollButton.interactable = _state == GameState.Playing;
            if (giveUpButton != null) giveUpButton.interactable = _state == GameState.Playing;
            if (restartButton != null) restartButton.interactable = _state == GameState.Results;

            UpdatePoisonDisplay();
            UpdateScoreDisplay();

            if (previousState != nextState)
            {
                _logger.Log("State changed from " + previousState + " to " + nextState + ".");
            }
        }

        private void UpdatePoisonDisplay()
        {
            SetText(
                poisonDiceText,
                $"Poison Dice: {(_state == GameState.Title ? "?" : _poisonDice.ToString())}"
            );
        }

        private void UpdateScoreDisplay()
        {
            SetText(scoreText, $"Score: {_score}");
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
