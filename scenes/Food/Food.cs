using Godot;
using System;

public class Food : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal] public delegate void Dead();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void Die() {
        EmitSignal(nameof(Dead));
        QueueFree();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
