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
        animatedSprite.Connect("animation_finished", this, nameof(_OnDying));
        animatedSprite.Play("Apparition", true);
	}

    public void _OnDying() {
        EmitSignal(nameof(Dead));
		QueueFree();
    }
}
