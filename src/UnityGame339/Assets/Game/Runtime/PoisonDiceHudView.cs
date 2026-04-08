using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class PoisonDiceHudView : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultsPanel;

        [Header("Labels")]
        [SerializeField] private TMP_Text poisonDiceLabel;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text gameplayHighScoreLabel;
        [SerializeField] private TMP_Text resultsHighScoreLabel;
        [SerializeField] private TMP_Text lastRollLabel;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private TMP_Text resultsHeaderLabel;
        [SerializeField] private TMP_Text finalScoreLabel;
        [SerializeField] private TMP_Text outcomeLabel;

        public void Initialize(
            GameObject title,
            GameObject gameplay,
            GameObject results,
            TMP_Text poison,
            TMP_Text score,
            TMP_Text gameplayHighScore,
            TMP_Text resultsHighScore,
            TMP_Text lastRoll,
            TMP_Text status,
            TMP_Text resultsHeader,
            TMP_Text finalScore,
            TMP_Text outcome)
        {
            titlePanel = title != null ? title : titlePanel;
            gameplayPanel = gameplay != null ? gameplay : gameplayPanel;
            resultsPanel = results != null ? results : resultsPanel;
            poisonDiceLabel = poison != null ? poison : poisonDiceLabel;
            scoreLabel = score != null ? score : scoreLabel;
            gameplayHighScoreLabel = gameplayHighScore != null ? gameplayHighScore : gameplayHighScoreLabel;
            resultsHighScoreLabel = resultsHighScore != null ? resultsHighScore : resultsHighScoreLabel;
            lastRollLabel = lastRoll != null ? lastRoll : lastRollLabel;
            statusLabel = status != null ? status : statusLabel;
            resultsHeaderLabel = resultsHeader != null ? resultsHeader : resultsHeaderLabel;
            finalScoreLabel = finalScore != null ? finalScore : finalScoreLabel;
            outcomeLabel = outcome != null ? outcome : outcomeLabel;
        }

        public void ShowState(PoisonDiceRoundState state)
        {
            if (titlePanel != null) titlePanel.SetActive(state == PoisonDiceRoundState.Title);
            if (gameplayPanel != null) gameplayPanel.SetActive(state == PoisonDiceRoundState.Playing);
            if (resultsPanel != null) resultsPanel.SetActive(state == PoisonDiceRoundState.Results);
        }

        public void Render(PoisonDiceScreenViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }
            
            ShowState(viewModel.RoundState);
            
            SetText(poisonDiceLabel, viewModel.PoisonLabel);
            SetText(scoreLabel, viewModel.ScoreLabel);
            SetText(gameplayHighScoreLabel, viewModel.HighScoreLabel);
            SetText(resultsHighScoreLabel, viewModel.HighScoreLabel);
            SetText(lastRollLabel, viewModel.LastRollLabel);
            SetText(statusLabel, viewModel.StatusLabel);
            SetText(resultsHeaderLabel, viewModel.ResultsHeader);
            SetText(finalScoreLabel, viewModel.FinalScoreLabel);
            SetText(outcomeLabel, viewModel.OutcomeLabel);
            SetTextColor(lastRollLabel, viewModel.LastRollColor);
            SetTextColor(statusLabel, viewModel.StatusColor);
            SetTextColor(outcomeLabel, viewModel.StatusColor);
        }

        private static void SetText(TMP_Text target, string value)
        {
            if (target != null)
            {
                target.text = value;
            }
        }

        private static void SetTextColor(TMP_Text target, Color value)
        {
            if (target != null)
            {
                target.color = value;
            }
        }
    }
}
