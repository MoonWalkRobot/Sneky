using Godot;
using System;

public class SettingsMenu : Control
{
    private InputChecker inputChecker;
	private SettingsButtonList buttonList;
	private Globals globals;

	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		inputChecker = GetNode<InputChecker>("InputChecker");
		buttonList = GetNode<SettingsButtonList>("CenterContainer/SettingsButtonList");
		inputChecker.Connect(nameof(InputChecker.Select), buttonList, nameof(ButtonList.NextFocusGrab));
		buttonList.GetNode<Button>("ThresholdContainer/Threshold").Connect("pressed", this, nameof(onButtonThresholdPressed));
		buttonList.GetNode<Button>("InvertControlContainer/InvertControl").Connect("pressed", this, nameof(onButtonInvertControlPressed));
		buttonList.GetNode<Button>("EnableShadersContainer/EnableShaders").Connect("pressed", this, nameof(onButtonEnableShadersPressed));
		buttonList.GetNode<Button>("Back").Connect("pressed", this, nameof(onButtonBackPressed));
	}

	private void onButtonThresholdPressed()
	{
		globals.InputThreshold = 50 + (globals.InputThreshold - 50 + 10) % 60;
	}

	private void onButtonInvertControlPressed()
	{
		globals.InvertControl = !globals.InvertControl;
	}

	private void onButtonEnableShadersPressed()
	{
		globals.EnableShaders = !globals.EnableShaders;
	}

	private void onButtonBackPressed()
	{
		GetParent().AddChild(ResourceLoader.Load<PackedScene>("res://scenes/MainMenu/MainMenu.tscn").Instance());
		QueueFree();
	}
}
