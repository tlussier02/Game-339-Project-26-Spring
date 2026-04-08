using TMPro;
using UnityEngine;

namespace Game.Runtime
{
    public class PoisonDiceResultsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerLabel;
        [SerializeField] private TMP_Text finalScoreLabel;
        [SerializeField] private TMP_Text outcomeLabel;

        public void Initialize(TMP_Text header, TMP_Text finalScore, TMP_Text outcome)
        {
            headerLabel = header != null ? header : headerLabel;
            finalScoreLabel = finalScore != null ? finalScore : finalScoreLabel;
            outcomeLabel = outcome != null ? outcome : outcomeLabel;
        }

        public void Render(PoisonDiceScreenViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            SetText(headerLabel, viewModel.ResultsHeader);
            SetText(finalScoreLabel, viewModel.FinalScoreLabel);
            SetText(outcomeLabel, viewModel.OutcomeLabel);
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
