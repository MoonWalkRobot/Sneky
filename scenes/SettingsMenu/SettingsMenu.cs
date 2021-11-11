using Godot;
using System;

public class SettingsMenu : Control
{
    private InputChecker _inputChecker;
	private SettingsButtonList _buttonList;
	private Globals _globals;

	public override void _Ready()
	{
		_globals = GetNode<Globals>("/root/Globals");
		_inputChecker = GetNode<InputChecker>("InputChecker");
		_buttonList = GetNode<SettingsButtonList>("CenterContainer/SettingsButtonList");
		_inputChecker.Connect(nameof(InputChecker.Select), _buttonList, nameof(ButtonList.NextFocusGrab));
		_buttonList.GetNode<Button>("ThresholdContainer/Threshold").Connect("pressed", this, "_onButtonThresholdPressed");
		_buttonList.GetNode<Button>("InvertControlContainer/InvertControl").Connect("pressed", this, "_onButtonInvertControlPressed");
		_buttonList.GetNode<Button>("Back").Connect("pressed", this, "_onButtonBackPressed");
	}

	private void _onButtonThresholdPressed()
	{
		_globals.InputThreshold = 50 + (_globals.InputThreshold - 50 + 10) % 100;
	}

	private void _onButtonInvertControlPressed()
	{
		_globals.InvertControl = !_globals.InvertControl;
	}

	private void _onButtonBackPressed()
	{
		GetParent().AddChild(ResourceLoader.Load<PackedScene>("res://scenes/MainMenu/MainMenu.tscn").Instance());
		QueueFree();
	}
}
