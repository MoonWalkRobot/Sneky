using Godot;
using System;

public class Bomb : Node2D
{
    [Signal] public delegate void Dead();
    private AnimatedSprite animatedSprite;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Connect("animation_finished", this, nameof(enableHitbox));
        GetNode("Area2D").SetProcess(false);
        animatedSprite.Play("Apparition");
    }

	public void Die() {
        animatedSprite.Connect("animation_finished", this, nameof(OnDying));
        animatedSprite.Play("Despawn");
	}

    public void OnDying() {
        EmitSignal(nameof(Dead));
		QueueFree();
    }

    private void enableHitbox()
    {
        GetNode("Area2D").SetProcess(true);
        animatedSprite.Disconnect("animation_finished", this, nameof(enableHitbox));
    }
}
