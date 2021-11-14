using Godot;
using System.Collections.Generic;

public class Body : Node2D
{
    public class Transition 
    {
        Transition(Vector2 position_, float angle_)
        {
            position = position_;
            angle = angle_;
        }
        Vector2 position;
        float angle;
    }

    public List<Transition> Transitions = new List<Transition>();
    private Tween _tween;

    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
    }

    public override void _Process(float delta)
    {
        
    }
}
