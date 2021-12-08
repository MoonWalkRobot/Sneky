using Godot;
using System;

public class GameOver : Control
{
    [Signal] public delegate void Retry();
    [Signal] public delegate void MainMenu();

    private Button retryButton;
    private Button mainMenuButton;
    private GameOverButtonList gameOverButtonList;
    private InputChecker inputChecker;

    public override void _Ready()
    {
        retryButton = GetNode<Button>("CenterContainer/GameOverButtonList/RetryButton");
        mainMenuButton = GetNode<Button>("CenterContainer/GameOverButtonList/MainMenuButton");
        gameOverButtonList = GetNode<GameOverButtonList>("CenterContainer/GameOverButtonList");
        inputChecker = GetNode<InputChecker>("InputChecker");

		inputChecker.Connect(nameof(InputChecker.Select), gameOverButtonList, nameof(GameOverButtonList.NextFocusGrab));

        retryButton.Connect("pressed", this, nameof(onRetryPressed));
        mainMenuButton.Connect("pressed", this, nameof(onMainMenuPressed));
    }

    private void onRetryPressed()
    {
        EmitSignal("Retry");
    }

    private void onMainMenuPressed()
    {
        EmitSignal("MainMenu");
    }
}
