using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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

        private void Awake()
        {
            ResolveReferences();
            RenderStoredResult();
        }

        public void SetRestartCallback(UnityAction restartAction)
        {
            ResolveReferences();

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

        public void RestartFromResultsScene()
        {
            var sceneName = FarmMatchResultsSession.RestartSceneName;
            FarmMatchResultsSession.Clear();

            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void Render(FarmMatchScreenViewModel viewModel)
        {
            ResolveReferences();

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

        private void ResolveReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = gameObject;
            }

            if (restartButton == null)
            {
                restartButton = GetComponentInChildren<Button>(true);
            }

            if (restartButtonLabel == null && restartButton != null)
            {
                restartButtonLabel = restartButton.GetComponentInChildren<TMP_Text>(true);
            }

            var labels = GetComponentsInChildren<TMP_Text>(true);
            for (var i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (label == null)
                {
                    continue;
                }

                if (currentScoreLabel == null && label.name == "FinalScoreText")
                {
                    currentScoreLabel = label;
                    continue;
                }

                if (highScoreLabel == null && label.name == "HighScoreText")
                {
                    highScoreLabel = label;
                    continue;
                }

                if (restartButtonLabel == null && label.transform.parent != null && label.transform.parent.name == "RestartButton")
                {
                    restartButtonLabel = label;
                }
            }
        }

        private void RenderStoredResult()
        {
            if (!FarmMatchResultsSession.HasResult)
            {
                return;
            }

            var result = FarmMatchResultsSession.LastResult;
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }

            SetText(headerLabel, result.DidLose ? "Game Over" : "Round Complete");
            SetText(currentScoreLabel, "Current Score: " + result.FinalScore);
            SetText(highScoreLabel, "High Score: " + result.HighScore);
            SetText(statusLabel, "Rounds Cleared: " + Mathf.Max(0, result.RoundNumber - 1));
            SetText(restartButtonLabel, "Restart");

            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartFromResultsScene);
                restartButton.interactable = true;
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
