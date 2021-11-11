using Godot;
using System;

public class SettingInvertControlLabel : Label
{
    private Globals _globals;
    public override void _Ready()
    {
        _globals = GetNode<Globals>("/root/Globals");
    }

    public override void _Process(float delta)
    {
        Text = _globals.InvertControl.ToString();
    }
}
