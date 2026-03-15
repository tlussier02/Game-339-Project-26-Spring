# Poison Dice Skeleton Setup

This is a minimal gameplay skeleton for the [Title] dice game.

Tracked skeleton scripts:

1. `PoisonDiceGameController.cs`
2. `PoisonDiceGameStateData.cs`
3. `PoisonDiceHudView.cs`
4. `PoisonDiceResultsView.cs`
5. `PoisonDiceSceneBootstrap.cs`

The controller is the only script with active gameplay logic right now.
The extra scripts are compile-safe scaffolding so the project opens cleanly and the
Poison Dice structure is visible in Unity before the scene is fully rewired.

Scene wiring target:

1. Open `Assets/Scenes/SampleScene.unity` in the Unity Editor.
2. Create a Canvas and three panels:
   - `TitlePanel` with a Start button.
   - `GameplayPanel` with text fields for Poison Dice, Score, Last Roll, and a Roll + Give Up button.
   - `ResultsPanel` with final score, status message, and a Restart button.
3. Add an empty GameObject named `PoisonDiceGame` and attach `PoisonDiceGameController`.
4. Optionally attach `PoisonDiceHudView`, `PoisonDiceResultsView`, and `PoisonDiceSceneBootstrap`
   as the next pass of scene organization work.
5. Drag UI elements into the controller fields:
   - Panels: `titlePanel`, `gameplayPanel`, `resultsPanel`
   - Texts: `poisonDiceText`, `scoreText`, `lastRollText`, `resultsHeaderText`, `finalScoreText`, `statusText`
   - Buttons: `startButton`, `rollButton`, `giveUpButton`, `restartButton`

Gameplay loop:

1. Start button sets the poison number.
2. Roll accumulates points while avoiding the poison value.
3. Give Up stores the current score and moves to Results.
4. Rolling poison immediately ends with final score = 0.
