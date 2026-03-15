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
        [SerializeField] private TMP_Text lastRollLabel;
        [SerializeField] private TMP_Text statusLabel;

        public void ShowState(PoisonDiceRoundState state)
        {
            if (titlePanel != null) titlePanel.SetActive(state == PoisonDiceRoundState.Title);
            if (gameplayPanel != null) gameplayPanel.SetActive(state == PoisonDiceRoundState.Playing);
            if (resultsPanel != null) resultsPanel.SetActive(state == PoisonDiceRoundState.Results);
        }

        public void Render(PoisonDiceGameStateData state)
        {
            SetText(poisonDiceLabel, state.RoundState == PoisonDiceRoundState.Title
                ? "Poison Dice: ?"
                : $"Poison Dice: {state.PoisonValue}");
            SetText(scoreLabel, $"Score: {state.CurrentScore}");
            SetText(lastRollLabel, state.LastRoll <= 0
                ? "Roll to begin"
                : $"Last Roll: {state.LastRoll}");
            SetText(statusLabel, state.DidBust
                ? "Poison hit. Round lost."
                : "Skeleton HUD ready for scene wiring.");
        }

        private static void SetText(TMP_Text target, string value)
        {
            if (target != null)
            {
                target.text = value;
            }
        }
    }
}
