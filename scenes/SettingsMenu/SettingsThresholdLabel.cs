using Godot;
using System;

public class SettingsThresholdLabel : Label
{
    private Globals _globals;

    public override void _Ready()
    {
        _globals = GetNode<Globals>("/root/Globals");
    }

    public override void _Process(float delta)
    {
        Text = _globals.InputThreshold.ToString();
    }
}
