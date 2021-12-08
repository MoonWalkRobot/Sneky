using Godot;
using System;

public class Food : Node2D
{
    public enum Octopus
    {
        basic,
        makeup,
        old,
        shiny
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
        GD.Print(num);
        if (num >= 90)
        {
            return Enum.GetNames(typeof(Octopus)).Length - 1;
        }
        else if (num >= 60)
        {
            return Enum.GetNames(typeof(Octopus)).Length - 2;
        }
        else if (num >= 30)
        {
            return Enum.GetNames(typeof(Octopus)).Length - 3;
        }
        else
        {
            return Enum.GetNames(typeof(Octopus)).Length - 4;
        }

    }

    public void Die()
    {
        EmitSignal(nameof(Dead), (Type == Octopus.shiny));
        QueueFree();
    }
}
