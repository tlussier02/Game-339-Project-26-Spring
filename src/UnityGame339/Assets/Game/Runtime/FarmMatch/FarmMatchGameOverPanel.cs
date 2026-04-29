using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchGameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text headerLabel;
        [SerializeField] private TMP_Text currentScoreLabel;
        [SerializeField] private TMP_Text highScoreLabel;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private TMP_Text restartButtonLabel;

        public void SetRestartCallback(UnityAction restartAction)
        {
            if (restartButton == null)
            {
                return;
            }

            restartButton.onClick.RemoveAllListeners();

            if (restartAction != null)
            {
                restartButton.onClick.AddListener(restartAction);
            }
        }

        public void Render(FarmMatchScreenViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            if (panelRoot != null)
            {
                panelRoot.SetActive(viewModel.ShowGameOverPanel);
            }

            SetText(headerLabel, viewModel.GameOverHeaderLabel);
            SetText(currentScoreLabel, viewModel.GameOverCurrentScoreLabel);
            SetText(highScoreLabel, viewModel.GameOverHighScoreLabel);
            SetText(statusLabel, viewModel.StatusLabel);
            SetText(restartButtonLabel, viewModel.RestartButtonLabel);

            if (restartButton != null)
            {
                restartButton.interactable = viewModel.CanRestart;
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
