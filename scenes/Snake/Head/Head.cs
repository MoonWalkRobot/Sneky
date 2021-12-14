using Godot;
using System;

public class Head : Node2D
{
    String NormalSprite = "res://scenes/Snake/Head/Snake_head1.png";
    String ReverseSprite = "res://scenes/Snake/Head/Snake_head_reverse.png";

    private const float OriginalSpeed = 200; //85
    private Vector2 speed = new Vector2(0, -OriginalSpeed);
    private float ReverseDuration = 0;
    private float rotationSpeed = 300; //120
    [Signal] public delegate void Reden();
    private ControlConverter controlConverter;
    private Body firstBody;
    private PackedScene queueScene = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Body/Body.tscn");
    private Timer timer;
    public float SnakeSpeed = OriginalSpeed;
    public const float SpeedRatio = 1f / 85f;
    public const float TransitionCount = 10;
    public float TransitionTime = 1 / (OriginalSpeed * SpeedRatio); // TODO: Not forget to update this when changing snake speed.
    public override void _Ready()
    {
        controlConverter = GetNode<ControlConverter>("/root/ControlConverter");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "_on_Timer_timeout");
        timer.OneShot = false;
        timer.Start(TransitionTime / TransitionCount); // TODO: Not forget to update this when changing snake speed.
        firstBody = queueScene.Instance<Body>();
        firstBody.Position = Position;
        firstBody.GetNode("Area2D").QueueFree();
        GetParent().CallDeferred("add_child", firstBody);
        GetParent().CallDeferred("move_child", firstBody, 0);
        GetNode<Area2D>("Area2D").Connect("area_entered", this, nameof(onArea2DEntered));
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
                    newBody.GetNode<Sprite>("Sprite").Visible = false;
                    newBody.GetNode<Sprite>("AltSprite").Visible = true;
                }
                newBody.Position = nextBody.Position;
                newBody.RotationDegrees = nextBody.RotationDegrees;
                GetParent().CallDeferred("add_child", newBody);
                GetParent().CallDeferred("move_child", newBody, 0);
                nextBody.NextBody = newBody;
                return;
            }
            else
            {
                nextBody = nextBody.NextBody;
            }
        }
    }

    public void ReverseRotation()
    {
        if (rotationSpeed < 0)
        {
            ReverseDuration = ReverseDuration + 600;
        }
        else
        {
            ReverseDuration = 600;
            rotationSpeed = -rotationSpeed;
            GetNode<Sprite>("Sprite").Texture = ResourceLoader.Load<Texture>(ReverseSprite);
        }
    }

    public void _OnReversedColor()
    {

    }

    private void _on_Timer_timeout()
    {
        firstBody.Transitions.Enqueue(new Body.Transition(Position, RotationDegrees));
    }

    public override void _Process(float delta)
    {
        if (rotationSpeed < 0)
        {
            if (ReverseDuration > 0)
            {
                ReverseDuration = ReverseDuration - 1;
            }
            else
            {
                ReverseDuration = 0;
                rotationSpeed = -rotationSpeed;
                GetNode<Sprite>("Sprite").Texture = ResourceLoader.Load<Texture>(NormalSprite);
            }
        }
        RotationDegrees += rotationSpeed * controlConverter.speed.y * delta;
        Move(delta);
    }

    private void onArea2DEntered(Area2D area)
    {
        Node parent = area.GetParent();
        if (parent is Food)
        {
            ((Food)parent).CallDeferred(nameof(Food.Die));
        }
        else if (parent is Bomb)
        {
            ((Bomb)parent).CallDeferred(nameof(Bomb.Die));
            GetParent<Snake>().TakeDamage();
        }
        else if (parent is Body)
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
        }
    }

    private void BorderEffect(Area2D area)
    {
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
}
