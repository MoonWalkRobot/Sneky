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
    public AnimatedSprite animatedSprite;
    [Signal] public delegate void Colored(string color);

    public override void _Ready()
    {
        head = GetParent().GetNode<Head>("Head");
        tween = GetNode<Tween>("Tween");
        timer = GetNode<Timer>("Timer");
        timer.OneShot = false;
        timer.Start(head.TransitionTime / Head.TransitionCount);
        timer.Connect("timeout", this, nameof(_onTimerTimeout));
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("Idle");
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
            if (NextBody != null)
            {
                NextBody.Transitions.Enqueue(transition);
            }
        }
    }

    public void onColored(string color)
    {
        Dictionary<string, string> animNames = new Dictionary<string, string>();
        animNames.Add("Red", "Reverse");
        animNames.Add("Green", "Reset");
        if (GetNode<Sprite>("AltSprite").Visible)
        {
            if (NextBody != null)
            {
                NextBody.onColored(color);
            }
        }
        else
        {
            animatedSprite.Play(animNames[color]);
            if (NextBody != null)
            {
                // GD.Print(NextBody);
                // animatedSprite.Connect("animation_finished", this, nameof(onSpreadColor));
                // Connect(nameof(Colored),NextBody,nameof(onColored));
                // EmitSignal(nameof(Colored));
                // Disconnect(nameof(Colored),this,nameof(onColored));
                NextBody.onColored(color);
            }
        }
    }

    public void onSpreadColor(string color)
    {
        NextBody.onColored(color);
        animatedSprite.Disconnect("animation_finished", this, nameof(onSpreadColor));
    }
}
