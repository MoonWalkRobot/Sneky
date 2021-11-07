using Godot;
using System;

public class InputChecker : Node
{
    [Signal] public delegate void Select();
    private ControlConverter _controlConverter;
    private Timer _timer;
    bool _doProcess = true;
    bool _timeoutUnpress = false;
    private float _threshold = 0.25f;
    public override void _Ready()
    {
        _controlConverter = GetNode<ControlConverter>("/root/ControlConverter");
        _timer = GetNode<Timer>("Timer");
        _timer.Connect("timeout", this, nameof(_onTimerTimeout));
    }

    public override void _Process(float delta)
    {
        if (!_doProcess)
        {
            return;
        }

        if (_controlConverter.speed.y > _threshold)
        {
            var a = new InputEventAction();
            a.Action = "ui_accept";
            a.Pressed = true;
            Input.ParseInputEvent(a);
            _timeoutUnpress = true;
            _wait();
        } else if (Math.Abs(_controlConverter.speed.y) > _threshold) {
            EmitSignal(nameof(Select));
            _wait();
        }
    }

    private void _wait()
    {
        _doProcess = false;
        _timer.Start();
    }

    private void _onTimerTimeout()
    {
        _doProcess = true;
        if (_timeoutUnpress) {
            var a = new InputEventAction();
            a.Action = "ui_accept";
            a.Pressed = false;
            Input.ParseInputEvent(a);
            _timeoutUnpress = false;
        }
    }
}
