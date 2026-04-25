using System;
using UnityEngine;

namespace Game.Runtime
{
    public sealed class PoisonDiceScreenViewModel
    {
        private readonly PoisonDiceGameModel _model;
        private readonly Color _safeRollColor;
        private readonly Color _poisonRollColor;
        private readonly Color _resultsWinColor;
        private readonly Color _resultsLoseColor;

        public PoisonDiceScreenViewModel(
            PoisonDiceGameModel model,
            Color safeRollColor,
            Color poisonRollColor,
            Color resultsWinColor,
            Color resultsLoseColor)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _safeRollColor = safeRollColor;
            _poisonRollColor = poisonRollColor;
            _resultsWinColor = resultsWinColor;
            _resultsLoseColor = resultsLoseColor;
        }

        public event Action Changed;

        public PoisonDiceRoundState RoundState => _model.State.RoundState;
        public bool ShowTitle => RoundState == PoisonDiceRoundState.Title;
        public bool ShowGameplay => RoundState == PoisonDiceRoundState.Playing;
        public bool ShowResults => RoundState == PoisonDiceRoundState.Results;

        public bool CanStart => ShowTitle;
        public bool CanRoll => ShowGameplay;
        public bool CanGiveUp => ShowGameplay;
        public bool CanRestart => ShowResults;

        public string PoisonLabel => ShowTitle
            ? "Poison Dice: ?"
            : $"Poison Dice: {_model.State.PoisonValue}";

        public string ScoreLabel => $"Score: {_model.State.CurrentScore}";
        public string HighScoreLabel => $"High Score: {_model.State.HighScore}";

        public string LastRollLabel
        {
            get
            {
                if (_model.State.LastRoll <= 0)
                {
                    return "Roll to begin";
                }

                return _model.State.DidBust
                    ? $"Rolled {_model.State.LastRoll} - poisoned!"
                    : $"Rolled {_model.State.LastRoll}";
            }
        }

        public string StatusLabel
        {
            get
            {
                if (ShowTitle)
                {
                    return "Press Start to begin.";
                }

                if (ShowGameplay)
                {
                    return _model.State.LastRoll <= 0
                        ? "Avoid this number to keep scoring."
                        : "Nice roll. Keep going or give up.";
                }

                return OutcomeLabel;
            }
        }

        public string ResultsHeader => _model.State.DidBust ? "Bust" : "Round Over";
        public string FinalScoreLabel => $"Final Score: {_model.State.FinalScore}";

        public string OutcomeLabel => _model.State.DidBust
            ? $"You hit the poison number ({_model.State.PoisonValue})."
            : "You chose to cash out.";

        public Color LastRollColor
        {
            get
            {
                if (_model.State.LastRoll <= 0)
                {
                    return Color.white;
                }

                return _model.State.DidBust ? _poisonRollColor : _safeRollColor;
            }
        }

        public Color StatusColor => ShowResults
            ? (_model.State.DidBust ? _resultsLoseColor : _resultsWinColor)
            : Color.white;

        public void Initialize()
        {
            _model.ResetToTitle();
            RaiseChanged();
        }

        public void StartGame()
        {
            _model.StartNewRound();
            RaiseChanged();
        }

        public void Roll()
        {
            _model.Roll();
            RaiseChanged();
        }

        public void GiveUp()
        {
            _model.GiveUp();
            RaiseChanged();
        }

        public void Restart()
        {
            _model.StartNewRound();
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            Changed?.Invoke();
        }
    }
}
