using Godot;
using System;

public class Fauna : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal] public delegate void Reaction();
    private AnimatedSprite animatedSprite;
    private float angle = 0;
    private Tween tween;
    const float distance = 1900; // ~= screen diagonal
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private const int FaunaSize = 4;
    public enum FaunaType {
        crab,
        eyes,
        clownfish,
        clapfish
    }

    public FaunaType Type;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        tween = GetNode<Tween>("Tween");
        animatedSprite.Play("idle_" + Type);
    }

    public void React() {
        if (Type == FaunaType.crab) {
            animatedSprite.Play("dive_crab");
            animatedSprite.Connect("animation_finished",this,nameof(onCrabReset));
        }
    }

    public void onCrabReset()
    {
        animatedSprite.Disconnect("animation_finished",this,nameof(onCrabReset));
        animatedSprite.Play("reset_dive_crab");
        animatedSprite.Connect("animation_finished",this,nameof(onCrabIdle));
    }

    public void onCrabIdle()
    {
        animatedSprite.Disconnect("animation_finished",this,nameof(onCrabIdle));
        animatedSprite.Play("idle_crab");
    }

    public void SetupFish()
    {
        Vector2 pos;
        Vector2 tmp = new Vector2(0, -distance);
        float angle;
        rng.Randomize();
        pos = new Vector2(rng.Randf() * 1600, rng.Randf() * 900);
        angle = rng.Randf() * 360;
        pos += tmp.Rotated(((float)Math.PI) * angle / 180);
        angle = 360 + (angle - 180) % 360;
        tween.InterpolateProperty(this, "position", pos, (tmp * 2).Rotated(((float)Math.PI) * angle / 180), 65);
        RotationDegrees = angle;
        tween.Connect("tween_all_completed", this, nameof(onTweenCompleted));
        tween.Start();
    }

    public void onTweenCompleted()
    {
        QueueFree();
    }
}
