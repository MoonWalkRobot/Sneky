using Godot;
using System;

public class Bomb : Node2D
{
    [Signal] public delegate void Dead();
    private AnimatedSprite animatedSprite;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("Apparition");
    }

	public void Die() {
		EmitSignal(nameof(Dead));
		QueueFree();
	}
}
