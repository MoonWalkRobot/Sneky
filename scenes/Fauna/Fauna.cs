using Godot;
using System;

public class Fauna : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal] public delegate void Reaction();
    private AnimatedSprite animatedSprite;
    Random rnd = new Random();

    private const int FaunaSize = 4;
    enum FaunaType {
        crab,
        eyes,
        clownfish,
        clapfish
    }

    private FaunaType type;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        type = (FaunaType) rnd.Next(FaunaSize);
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("idle_" + type);
    }

    public void React() {
        if (type == FaunaType.crab) {
            animatedSprite.Play("dive_crab");
            
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
