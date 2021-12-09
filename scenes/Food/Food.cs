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
            int type = (num >= 50) ? (int)Octopus.shiny : (int)Octopus.shiny;
            return type;
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
}
