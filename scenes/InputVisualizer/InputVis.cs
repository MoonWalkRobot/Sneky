using Godot;
using System;

public class InputVis : Godot.ProgressBar
{

    private ControlConverter _controlConverter;

    public override void _Ready()
    {
        _controlConverter = GetNode<ControlConverter>("/root/ControlConverter");
        Set("z", 0);
    }
    public override void _Process(float delta) 
    { 
        Value = _controlConverter.speed.y;
    }
}
