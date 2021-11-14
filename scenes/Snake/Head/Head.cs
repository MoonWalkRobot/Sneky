using Godot;
using System;

public class Head : Node2D
{
    private Vector2 speed = new Vector2(0, -85);
    private float rotationSpeed = 120;
    private ControlConverter controlConverter;
    private Body firstBody;
    private PackedScene queueScene = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Body/Body.tscn");
    private Timer timer;

    public override void _Ready()
    {
        controlConverter = GetNode<ControlConverter>("/root/ControlConverter");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "_on_Timer_timeout");
        timer.OneShot = false;
        timer.Start(0.1f);
        firstBody = queueScene.Instance<Body>();
        firstBody.GetNode("Area2D").QueueFree();
        GetParent().CallDeferred("add_child", firstBody);
        GetNode<Area2D>("Area2D").Connect("area_entered", this, nameof(onArea2DEntered));
    }

    public void Move(float delta)
    {
        Position += speed.Rotated(((float) Math.PI) * RotationDegrees / 180) * delta;
    }

    public void AddBody() {
        Body nextBody = firstBody;
        while(true) {
            if (nextBody.NextBody == null) {
                Body newBody = queueScene.Instance<Body>();
                GetParent().AddChild(newBody);
                nextBody.NextBody = newBody;
                return;
            } else {
                nextBody = nextBody.NextBody;
            }
        }
    }

    private void _on_Timer_timeout() {
        firstBody.Transitions.Enqueue(new Body.Transition(Position, RotationDegrees));
    }

    public override void _Process(float delta)
    {
        RotationDegrees += rotationSpeed * controlConverter.speed.y * delta;
        Move(delta);
    }

    private void onArea2DEntered(Area2D area)
    {
        GD.Print("Collided with " + area.GetParent().Name);
        // TODO DEAD
    }
}
