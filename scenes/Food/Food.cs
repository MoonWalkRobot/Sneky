using Godot;
using System;

public class Food : Node2D
{
    public enum Octopus
    {
        basic,
        shiny,
        makeup,
        old
    }
    [Signal] public delegate void Dead(bool alt);
    private AnimatedSprite animatedSprite;
    private Random rnd = new Random();
    public Octopus Type;

    public override void _Ready()
    {
        Type = (Octopus)rnd.Next(Enum.GetNames(typeof(Octopus)).Length);
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("idle_" + Type);
    }

    public void Die()
    {
        EmitSignal(nameof(Dead), (Type == Octopus.shiny));
        QueueFree();
    }
}
