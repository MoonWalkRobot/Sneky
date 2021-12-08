using Godot;
using System;

public class ControlConverter : Node
{

    public Vector2 speed;
    private Globals _globals;
    private const float _decay = 10;
    private bool _doDecay = false;

    public override void _Ready()
    {
        _globals = GetNode<Globals>("/root/Globals");
        PauseMode = Node.PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        if (!_doDecay)
        {
            speed.y = F(speed.y, 0);
        }
        else
        {
            _doDecay = true;
        }

        // DEBUG TESTING REMOVE LATER
        if (Input.IsKeyPressed((int)KeyList.Q))
        {
            speed.y = -1f;
        }
        else if (Input.IsKeyPressed((int)KeyList.S))
        {
            speed.y = -0.75f;
        }
        else if (Input.IsKeyPressed((int)KeyList.S))
        {
            speed.y = -0.75f;
        }
        else if (Input.IsKeyPressed((int)KeyList.D))
        {
            speed.y = -0.50f;
        }
        else if (Input.IsKeyPressed((int)KeyList.F))
        {
            speed.y = -0.25f;
        }
        else if (Input.IsKeyPressed((int)KeyList.J))
        {
            speed.y = 0.25f;
        }
        else if (Input.IsKeyPressed((int)KeyList.K))
        {
            speed.y = 0.50f;
        }
        else if (Input.IsKeyPressed((int)KeyList.L))
        {
            speed.y = 0.75f;
        }
        else if (Input.IsKeyPressed((int)KeyList.M))
        {
            speed.y = 1f;
        }
        // DEBUG
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && mouseMotion.Position != GetViewport().Size / 2)
        {
            float p = mouseMotion.Relative.y;
            if (_globals.InvertControl)
            {
                p = -p;
            }
            float s = Mathf.Clamp(p, -_globals.InputThreshold, _globals.InputThreshold);
            float n = s / _globals.InputThreshold;
            speed.y = F(speed.y, n);
            _doDecay = false;
        }

    }

    private float F(float x, float y)
    {
        float n = y - x;
        return x + (n * _decay / 100);
    }
}
