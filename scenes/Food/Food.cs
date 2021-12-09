using Godot;
using System;

public class Food : Node2D
{
    public enum Octopus
    {
        basic,
        makeup,
        old,
        shiny,
        reverse,
        alt
    }
    [Signal] public delegate void Dead(bool alt);
    private AnimatedSprite animatedSprite;
    private Random rnd = new Random();
    public Octopus Type;
    private float duration = 600;

    public override void _Ready()
    {
        Type = (Octopus)SelectOctopus();
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("idle_" + Type);
    }

    private int SelectOctopus()
    {
        int num = rnd.Next(100);
        if (Type == Octopus.alt)
        {
            if (num >= 50)
            {
                return (int)Octopus.shiny;
            }
            else
            {
                return (int)Octopus.reverse;
            }
        }
        else
        {
            if (num >= 66)
            {
                return (int)Octopus.old;
            }
            else if (num >= 33)
            {
                return (int)Octopus.makeup;
            }
            else
            {
                return (int)Octopus.basic;
            }
        }
    }

    public void Die()
    {
        EmitSignal(nameof(Dead), (Type == Octopus.shiny), (Type == Octopus.reverse));
        QueueFree();
    }

    override public void _Process(float delta) {
        if(Type == Octopus.shiny || Type == Octopus.reverse) {
            duration = duration - 1;
            if (duration <= 0) {
                QueueFree();
            }
        }
    }
}
