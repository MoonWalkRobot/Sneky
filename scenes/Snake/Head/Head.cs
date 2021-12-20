using Godot;
using System;

public class Head : Node2D
{
    private const float OriginalSpeed = 200; //85
    private Vector2 speed = new Vector2(0, -OriginalSpeed);
    private float ReverseDuration = 0;
    private float rotationSpeed = 300; //120
    private ControlConverter controlConverter;
    private Body firstBody;
    private PackedScene queueScene = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Body/Body.tscn");
    private Timer timer;
    private Timer invincibilityTimer;
    private Timer reverseTimer;
    private AnimatedSprite animatedSprite;
    private AnimationPlayer animationPlayer;
    private bool isInvincible = false;
    private bool reversed = false;
    [Signal] public delegate void Colored(string color);

    public float SnakeSpeed = OriginalSpeed;
    public const float SpeedRatio = 1f / 85f;
    public const float TransitionCount = 10;
    public float TransitionTime = 1 / (OriginalSpeed * SpeedRatio); // TODO: Not forget to update this when changing snake speed.
    public bool directionBlocked = false;
    public Timer directionBlockedTimer;
    private float _rotationDegrees;
    private float BLOCKED_DURATION = 0.5f;
    [Signal] public delegate void BorderHit();
    public override void _Ready()
    {
        controlConverter = GetNode<ControlConverter>("/root/ControlConverter");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "_on_Timer_timeout");
        timer.OneShot = false;
        timer.Start(TransitionTime / TransitionCount); // TODO: Not forget to update this when changing snake speed.
        invincibilityTimer = GetNode<Timer>("InvincibilityTimer");
        invincibilityTimer.Connect("timeout", this, nameof(onInvincibilityTimerTimeout));
        reverseTimer = GetNode<Timer>("ReverseTimer");
        reverseTimer.Connect("timeout", this, nameof(onReverseTimerTimeout));
        directionBlockedTimer = GetNode<Timer>("DirectionBlockedTimer");
        directionBlockedTimer.Connect("timeout", this, nameof(onDirectionBlockedTimeout));
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        firstBody = queueScene.Instance<Body>();
        firstBody.Position = Position;
        firstBody.GetNode("Area2D").QueueFree();
        Connect(nameof(Colored), firstBody, nameof(Body.onColored));
        GetParent().CallDeferred("add_child", firstBody);
        GetParent().CallDeferred("move_child", firstBody, 0);
        GetNode<Area2D>("Area2D").Connect("area_entered", this, nameof(onArea2DEntered));
        _rotationDegrees = RotationDegrees;
    }

    public void Move(float delta)
    {
        Position += speed.Rotated(((float)Math.PI) * RotationDegrees / 180) * delta;
    }

    public void AddBody(bool alt = false)
    {
        Body nextBody = firstBody;
        while (true)
        {
            if (nextBody.NextBody == null)
            {
                Body newBody = queueScene.Instance<Body>();
                if (alt)
                {
                    newBody.GetNode<AnimatedSprite>("AnimatedSprite").Visible = false;
                    newBody.GetNode<Sprite>("AltSprite").Visible = true;
                }
                newBody.Position = nextBody.Position;
                newBody.RotationDegrees = nextBody.RotationDegrees;
                GetParent().CallDeferred("add_child", newBody);
                GetParent().CallDeferred("move_child", newBody, 0);
                nextBody.NextBody = newBody;
                nextBody.Connect("Colored", newBody, nameof(Body.onColored));
                return;
            }
            else
            {
                nextBody = nextBody.NextBody;
            }
        }
    }

    public void BecomeInvincible(float duration)
    {
        invincibilityTimer.Start(duration);
        animationPlayer.Play("Invincible");
        isInvincible = true;
    }

    private void onInvincibilityTimerTimeout()
    {
        animationPlayer.Stop();
        Modulate = new Color("FFFFFF");
        isInvincible = false;
    }

    private void _on_Timer_timeout()
    {
        firstBody.Transitions.Enqueue(new Body.Transition(Position, RotationDegrees));
    }

    public override void _Process(float delta)
    {
        RotationDegrees += rotationSpeed * controlConverter.speed.y * delta;
        Move(delta);
        if (directionBlocked)
        {
            RotationDegrees = _rotationDegrees;
        }
    }

    private void onArea2DEntered(Area2D area)
    {
        Node parent = area.GetParent();
        if (parent is Food)
        {
            ((Food)parent).CallDeferred(nameof(Food.Die));
        }
        else if (parent is Bomb && !isInvincible)
        {
            ((Bomb)parent).CallDeferred(nameof(Bomb.Die));
            GetParent<Snake>().TakeDamage();
        }
        else if (parent is Body && !isInvincible)
        {
            GetParent<Snake>().TakeDamage();
        }
        else if (parent is Fauna)
        {
            ((Fauna)parent).CallDeferred(nameof(Fauna.React));
        }
        else if (area is Border)
        {
            BorderEffect(area);
            BlockDirection();
        }
    }

    private void BlockDirection()
    {
        directionBlocked = true;
        _rotationDegrees = RotationDegrees;
        directionBlockedTimer.Start(BLOCKED_DURATION);
    }

    private void BorderEffect(Area2D area)
    {
        EmitSignal(nameof(BorderHit));
        Vector2 p1 = Position;
        Vector2 p2 = Position + speed.Rotated(((float)Math.PI) * RotationDegrees / 180) * 10;
        float tetaRadV = 2 * (p1.x - p2.x) / (float)Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        float tetaRadH = 2 * (p1.y - p2.y) / (float)Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));

        switch (area.GetType().ToString())
        {
            case "Top":
                RotationDegrees += (float)(Math.Sign(p2.x - p1.x) * tetaRadH * (180 / (float)Math.PI));
                break;
            case "Bottom":
                RotationDegrees += (float)(Math.Sign(p2.x - p1.x) * tetaRadH * (180 / (float)Math.PI));
                break;
            case "Left":
                RotationDegrees += (float)(Math.Sign(p1.y - p2.y) * tetaRadV * (180 / (float)Math.PI));
                break;
            case "Right":
                RotationDegrees += (float)(Math.Sign(p1.y - p2.y) * tetaRadV * (180 / (float)Math.PI));
                break;
        }
    }

    public void onDirectionBlockedTimeout()
    {
        directionBlocked = false;
    }

    public void Reverse(bool reverse)
    {
        reversed = reverse;
        if (reverse)
        {
            rotationSpeed = -Math.Abs(rotationSpeed);
            reverseTimer.Start(10);
            EmitSignal(nameof(Colored), "Red");
        }
        else
        {
            rotationSpeed = Math.Abs(rotationSpeed);
            animatedSprite.Play("Idle");
        }
    }

    private void onReverseTimerTimeout()
    {
        Reverse(false);
        EmitSignal(nameof(Colored), "Green");
    }



    public void PlayAnimation(string animation)
    {
        if (reversed)
        {
            animation = "Reverse" + animation;
        }
        if (!animation.Contains("Idle") && !animatedSprite.IsConnected("animation_finished", this, nameof(onAnimatedSpriteAnimationFinished)))
        {
            animatedSprite.Connect("animation_finished", this, nameof(onAnimatedSpriteAnimationFinished));
        }
        animatedSprite.Play(animation);
    }

    private void onAnimatedSpriteAnimationFinished()
    {
        animatedSprite.Disconnect("animation_finished", this, nameof(onAnimatedSpriteAnimationFinished));
        PlayAnimation("Idle");
    }
}
