# Poison Dice Skeleton Setup

This is a minimal gameplay skeleton for the [Title] dice game:

1. Open `Assets/Scenes/SampleScene.unity` in the Unity Editor.
2. Create a Canvas and three panels:
   - `TitlePanel` with a Start button.
   - `GameplayPanel` with text fields for Poison Dice, Score, Last Roll, and a Roll + Give Up button.
   - `ResultsPanel` with final score, status message, and a Restart button.
3. Add an empty GameObject named `PoisonDiceGame` and attach `PoisonDiceGameController`.
4. Drag each UI element into the corresponding serialized field:
   - Panels: `titlePanel`, `gameplayPanel`, `resultsPanel`
   - Texts: `poisonDiceText`, `scoreText`, `lastRollText`, `resultsHeaderText`, `finalScoreText`, `statusText`
   - Buttons: `startButton`, `rollButton`, `giveUpButton`, `restartButton`
5. Enable compilation by adding `POISON_DICE_GAME_ENABLED` to Project Settings > Player > Scripting Define Symbols.
6. Start play:
   - Start button sets the poison number.
   - Roll accumulates points while avoiding the poison value.
   - Give Up stores the current score and moves to Results.
   - Rolling poison immediately ends with final score = 0.

The script is intentionally lightweight and includes no extra game mode, audio, or art systems so it is easy to expand next.
