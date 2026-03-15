using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class PoisonDiceResultsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerLabel;
        [SerializeField] private TMP_Text finalScoreLabel;
        [SerializeField] private TMP_Text outcomeLabel;

        public void Render(PoisonDiceGameStateData state)
        {
            SetText(headerLabel, state.DidBust ? "Bust" : "Round Over");
            SetText(finalScoreLabel, $"Final Score: {state.FinalScore}");
            SetText(outcomeLabel, state.DidBust
                ? "You rolled the poison number."
                : "You cashed out safely.");
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
