using System;

namespace Game.Runtime
{
    public enum PoisonDiceRoundState
    {
        Title,
        Playing,
        Results
    }

    [Serializable]
    public class PoisonDiceGameStateData
    {
        public PoisonDiceRoundState RoundState = PoisonDiceRoundState.Title;
        public int PoisonValue;
        public int CurrentScore;
        public int FinalScore;
        public int LastRoll;
        public bool DidBust;
        public int HighScore;

        public void ResetToTitle()
        {
            RoundState = PoisonDiceRoundState.Title;
            PoisonValue = 0;
            CurrentScore = 0;
            FinalScore = 0;
            LastRoll = 0;
            DidBust = false;
        }

        public void ResetForNewRound(int poisonValue)
        {
            RoundState = PoisonDiceRoundState.Playing;
            PoisonValue = poisonValue;
            CurrentScore = 0;
            FinalScore = 0;
            LastRoll = 0;
            DidBust = false;
        }

        public void RecordSafeRoll(int roll)
        {
            LastRoll = roll;
            CurrentScore += roll;
        }

        public void RecordBust(int roll)
        {
            LastRoll = roll;
            FinalScore = 0;
            DidBust = true;
            RoundState = PoisonDiceRoundState.Results;
        }

        public void CashOut()
        {
            FinalScore = CurrentScore;
            DidBust = false;
            RoundState = PoisonDiceRoundState.Results;

            if (FinalScore > HighScore)
            {
                HighScore = FinalScore;
            }
        }
    }
}
