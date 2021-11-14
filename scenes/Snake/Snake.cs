using Godot;
using System;

public class Snake : Node
{
    private Head head;

    public override void _Ready()
    {
        head = GetNode<Head>("Head");
        head.AddBody();
        head.AddBody();
        head.AddBody();
        head.AddBody();
        head.AddBody();
        head.AddBody();
        head.AddBody();
    }

}
