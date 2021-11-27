using Godot;
using System;

public class Food : Node2D
{
    [Signal] public delegate void Dead();
    private AnimatedSprite animatedSprite;
    Random rnd = new Random();

    private const int FamilySize = 4;
    enum Octopus {
        basic,
        shiny,
        makeup,
        old
    }


    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("idle_" + (Octopus) rnd.Next(FamilySize));
    }

    public void Die()
    {
        EmitSignal(nameof(Dead));
        QueueFree();
    }
}
