using Godot;
using System;

public class MainMenu : Control
{
    private InputChecker _inputChecker;
    private ButtonList _buttonList;

    public override void _Ready()
    {
        _inputChecker = GetNode<InputChecker>("InputChecker");
        _buttonList = GetNode<ButtonList>("CenterContainer/ButtonList");

        _inputChecker.Connect(nameof(InputChecker.Select), _buttonList, nameof(ButtonList.NextFocusGrab));
        _buttonList.GetNode<Button>("Play").Connect("pressed", this, "_onButtonPlayPressed");
        _buttonList.GetNode<Button>("Settings").Connect("pressed", this, "_onButtonSettingsPressed");
        _buttonList.GetNode<Button>("Quit").Connect("pressed", this, "_onButtonQuitPressed");
    }

    private void _onButtonPlayPressed()
    {
        GD.Print("Play scene");
        //Play Scene.
    }

    private void _onButtonSettingsPressed()
    {
        GD.Print("Settings menu");
        //Settings menu stuff.
    }

    private void _onButtonQuitPressed()
    {
        GD.Print("Quit game");
        //Quit game stuff.
    }

}
