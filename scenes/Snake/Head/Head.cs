using Godot;
using System;

public class Head : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Vector2 speed = new Vector2(0, -40);
    private float rotationSpeed = 120;
    private ControlConverter controlConverter;
    private Body firstBody;
    private PackedScene queueScene = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Body/Body.tscn");
    private Timer timer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        controlConverter = GetNode<ControlConverter>("/root/ControlConverter");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "_on_Timer_timeout");
        timer.OneShot = false;
        timer.Start(2);
        firstBody = queueScene.Instance<Body>();
        CallDeferred("add_child", firstBody);
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
        GD.Print("FDP");
        AddBody();
        firstBody.Transitions.Enqueue(new Body.Transition(Position, RotationDegrees));
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        RotationDegrees += rotationSpeed * controlConverter.speed.y * delta;
        Move(delta);
    }
}
