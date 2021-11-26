using Godot;
using System.Collections.Generic;

public class Body : Node2D
{
    public class Transition
    {
        public Transition(Vector2 position_, float angle_)
        {
            position = position_;
            angle = angle_;
        }
        public Vector2 position;
        public float angle;
    }
    public Queue<Transition> Transitions = new Queue<Transition>();
    private Tween tween;
    private Timer timer;
    private Head head;
    public Body NextBody;

    public override void _Ready()
    {
        head = GetParent().GetNode<Head>("Head");
        tween = GetNode<Tween>("Tween");
        timer = GetNode<Timer>("Timer");
        timer.OneShot = false;
        timer.Start(head.TransitionTime / Head.TransitionCount);
        timer.Connect("timeout", this, nameof(_onTimerTimeout));
    }

    public override void _Process(float delta)
    {

    }

    private void _onTimerTimeout()
    {
        if (Transitions.Count >= Head.TransitionCount)
        {
            Transition transition = Transitions.Dequeue();
            tween.InterpolateProperty(this, "position", Position, transition.position, head.TransitionTime / Head.TransitionCount);
            tween.InterpolateProperty(this, "rotation_degrees", RotationDegrees, transition.angle, head.TransitionTime / Head.TransitionCount);
            tween.Start();
            if (NextBody != null) {
                NextBody.Transitions.Enqueue(transition);
            }
        }
    }
}
