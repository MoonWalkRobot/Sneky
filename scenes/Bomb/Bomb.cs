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
        GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
        animatedSprite.Play("Apparition");
    }

    public void Die()
    {
        GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
        if (!animatedSprite.IsConnected("animation_finished", this, nameof(OnDying)))
        {
            animatedSprite.Connect("animation_finished", this, nameof(OnDying));
        }
        animatedSprite.Play("Despawn");
    }

    public void OnDying()
    {
        EmitSignal(nameof(Dead));
        QueueFree();
    }

    private void enableHitbox()
    {
        GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = false;
        animatedSprite.Disconnect("animation_finished", this, nameof(enableHitbox));
    }
}
