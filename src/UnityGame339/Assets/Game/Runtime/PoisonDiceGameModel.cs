using System;
using System.Collections;
using UnityEngine;

namespace Game.Runtime
{
    public sealed class PoisonDiceGameModel
    {
        private readonly Func<int> _rollDie;
        private PoisonDiceGameStateData _state;
        private IGameLogger _logger;
        private PoisonDiceAnimations _animation;

        public PoisonDiceGameModel(Func<int> rollDie, IGameLogger logger = null)
        {
            _rollDie = rollDie ?? throw new ArgumentNullException(nameof(rollDie));
            _logger = logger ?? NullGameLogger.Instance;
            _state = new PoisonDiceGameStateData();
            _state.ResetToTitle();
        }

        public PoisonDiceGameStateData State => _state;

        public void SetLogger(IGameLogger logger)
        {
            _logger = logger ?? NullGameLogger.Instance;
        }

        public void ResetToTitle()
        {
            _state.ResetToTitle();
            _logger.Log("Returned to the title screen.");
        }

        public void StartNewRound()
        {
            var poisonValue = _rollDie();
            _state.ResetForNewRound(poisonValue);
            _logger.Log("Started a new round. Poison number is " + poisonValue + ". Score reset to 0.");
        }

        public void Roll()
        {
            if (_state.RoundState != PoisonDiceRoundState.Playing)
            {
                _logger.LogWarning("Ignored roll input because the game is in state " + _state.RoundState + ".");
                return;
            }
            
            var roll = _rollDie();
            if (roll == _state.PoisonValue)
            {
                _state.RecordBust(roll);
                _logger.LogWarning("Player rolled " + roll + ", matched the poison number, and lost the round.");
                return;
            }

            _state.RecordSafeRoll(roll);
            _logger.Log("Player rolled " + roll + " safely. Score is now " + _state.CurrentScore + ".");
        }

        public void GiveUp()
        {
            if (_state.RoundState != PoisonDiceRoundState.Playing)
            {
                _logger.LogWarning("Ignored give-up input because the game is in state " + _state.RoundState + ".");
                return;
            }

            _state.CashOut();
            _logger.Log("Player cashed out with " + _state.FinalScore + " points.");
        }
    }
}
