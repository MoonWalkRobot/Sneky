using Godot;
using System.Collections.Generic;

public class Body : Node2D
{
    public class Transition
    {
        Transition(Vector2 position_, float angle_)
        {
            position = position_;
            angle = angle_;
        }
        public Vector2 position;
        public float angle;
    }
    public Queue<Transition> Transitions = new Queue<Transition>();
    private Tween _tween;
    private Timer _timer;
    public Body NextBody;

    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = false;
        _timer.Start(0.1f);
        _timer.Connect("timeout", this, nameof(_onTimerTimeout));
    }

    public override void _Process(float delta)
    {

    }

    private void _onTimerTimeout()
    {
        if (Transitions.Count >= 10)
        {
            Transition transition = Transitions.Dequeue();
            _tween.InterpolateProperty(this, "position", Position, transition.position, 0.1f);
            _tween.InterpolateProperty(this, "rotation_degrees", RotationDegrees, transition.angle, 0.1f);
            _tween.Start();
            if (NextBody != null) {
                NextBody.Transitions.Enqueue(transition);
            }
        }
    }
}
