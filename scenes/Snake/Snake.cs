using Godot;
using System;

public class Snake : Node2D
{
    private Head head;

    public override void _Ready()
    {
        head = GetNode<Head>("Head");
    }

}
