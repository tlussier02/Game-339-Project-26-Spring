# Poison Dice Skeleton Setup

This is a minimal gameplay skeleton for the [Title] dice game.

Tracked skeleton scripts:

1. `PoisonDiceGameController.cs`
2. `GameLogger.cs`
3. `PoisonDiceGameStateData.cs`
4. `PoisonDiceGameModel.cs`
5. `PoisonDiceScreenViewModel.cs`
6. `PoisonDiceHudView.cs`
7. `PoisonDiceResultsView.cs`
8. `PoisonDiceSceneBootstrap.cs`

The project now uses a lite MVVM split:

1. `PoisonDiceGameModel` owns the round state and gameplay rules.
2. `PoisonDiceScreenViewModel` exposes UI-ready labels, colors, panel state, and button state.
3. `PoisonDiceHudView` and `PoisonDiceResultsView` render data only.
4. `PoisonDiceGameController` remains the Unity composition root so existing inspector links still work.

Scene wiring target:

1. Open `Assets/Scenes/GameScreen.unity` in the Unity Editor.
2. Create or keep a Canvas with three panels:
   - `TitlePanel` with a Start button.
   - `GameplayPanel` with text fields for Poison Dice, Score, Last Roll, and a Roll + Give Up button.
   - `ResultsPanel` with final score, status message, and a Restart button.
3. Keep or create an empty GameObject named `PoisonDiceGame` with `PoisonDiceGameController`.
4. Attach `PoisonDiceHudView` to the same GameObject if you want the dedicated view component active.
5. Optionally attach `PoisonDiceResultsView` if you want the results panel rendered by a separate view component.
6. Drag UI elements into the controller fields:
   - Panels: `titlePanel`, `gameplayPanel`, `resultsPanel`
   - Texts: `poisonDiceText`, `scoreText`, `lastRollText`, `resultsHeaderText`, `finalScoreText`, `statusText`
   - Buttons: `startButton`, `rollButton`, `giveUpButton`, `restartButton`
7. The controller now initializes the view components from its own serialized references at runtime,
   so you do not need to duplicate the same links onto `PoisonDiceHudView` or `PoisonDiceResultsView`
   unless you want those components to carry their own inspector wiring.
8. Leave `enableDebugLogs` turned on if you want Unity Console output showing state changes,
   poison number selection, rolls, busts, and cash-outs during play.

Gameplay loop:

1. Start button sets the poison number.
2. Roll accumulates points while avoiding the poison value.
3. Give Up stores the current score and moves to Results.
4. Restart returns to the title screen so the player can begin another round.
5. Rolling poison immediately ends with final score = 0.
