using System;
using System.Collections.Generic;
using Godot;

public class MainGame : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    // TileMap tl;

    // const int SNAKE = 0;
    Head head;
    List<Body> body = new List<Body>();

    List<Vector2> snakeBody = new List<Vector2>();

    // Vector2 snakeDirection = new Vector2(1,0);
    // Vector2 speed = new Vector2(1,0);
    float rotationDegree = 0f;

    float speed = 1f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        snakeBody.Add(new Vector2(0, 0));
        snakeBody.Add(new Vector2(50, 0));
        snakeBody.Add(new Vector2(300, 0));
        // tl = GetNode<TileMap>("Map");

        // int test = GetNode<TileMap>("Map").GetCell(0,0);
        // tl.SetCell(0,0,0,false,false,false);
        // GD.Print(test);
        // Move();
        // DrawSnake();
        InitSneki();
        GD.Print("TOTO");

        // Snake snake = (Snake)((PackedScene)ResourceLoader.Load("res://src/scenes/Snake/Snake.tscn")).Instance();
    }

    // public void Move() {
    //     DeleteTiles();
    //     var bodyCopy = snakeBody.GetRange(0, snakeBody.Count - 1);
    //     var newHead = bodyCopy[0] + snakeDirection;
    //     bodyCopy.Insert(0,newHead);
    //     snakeBody = bodyCopy;
    // }
    public void InitSneki()
    {
        Head head =
            (
            Head
            )((PackedScene)
            ResourceLoader.Load("res://scenes/Snake/Head/Head.tscn"))
                .Instance();
        head.SetPosition(snakeBody[0]);
        AddChild (head);
        for (int i = 1; i < snakeBody.Count; i++)
        {
            Body bodyPart =
                (
                Body
                )((PackedScene)
                ResourceLoader.Load("res://scenes/Snake/Body/Body.tscn"))
                    .Instance();
            bodyPart.SetPosition(snakeBody[i]);
            AddChild (bodyPart);
            body.append(bodyPart);
        }
    }

    // public void DrawSnake() {
    //     foreach (Vector2 snakePiece in snakeBody){
    //         tl.SetCell((int) snakePiece.x,(int) snakePiece.y,SNAKE,false,false,false);
    //     }
    // }
    // public void DeleteTiles() {
    //     foreach(Vector2 snakePiece in snakeBody) {
    //         tl.SetCell((int) snakePiece.x, (int) snakePiece.y, -1);
    //     }
    // }
    // public override void _Input(InputEvent inputEvent)
    // {
    //     if (inputEvent.IsActionPressed("ui_up"))
    //     {
    //         snakeDirection = new Vector2(0,-1);
    //     }
    //         if (inputEvent.IsActionPressed("ui_down"))
    //     {
    //         snakeDirection = new Vector2(0,1);
    //     }
    //         if (inputEvent.IsActionPressed("ui_right"))
    //     {
    //         snakeDirection = new Vector2(1,0);
    //     }
    //         if (inputEvent.IsActionPressed("ui_left"))
    //     {
    //         snakeDirection = new Vector2(-1,0);
    //     }
    // }
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("rot_1"))
        {
            rotationDegree = 1f * 90;
        }
        if (inputEvent.IsActionPressed("rot_05"))
        {
            rotationDegree = 0.5f * 90;
        }
        if (inputEvent.IsActionPressed("rot_m05"))
        {
            rotationDegree = -0.5f * 90;
            GD.Print("blbl");
        }
        if (inputEvent.IsActionPressed("rot_m1"))
        {
            rotationDegree = -1f * 90;
        }
    }

    public void Move(float delta)
    {
        var bodyCopy = snakeBody.GetRange(0, snakeBody.Count - 1);
        var newHead = bodyCopy[0] + new Vector2((speed * delta * (float) Math.Cos(Math.PI * rotationDegree / 180)), (speed * delta * (float) Math.Sin(Math.PI * rotationDegree / 180)));
        bodyCopy.Insert(0,newHead);
        snakeBody = bodyCopy;
        
    }

    public void _on_Timer_timeout()
    {
        // Move();
        // DeleteTiles();
        // DrawSnake();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Move(delta);
        // Position = Position + (speed.rotated(RADIANT(RotationDegree)) * delta);
    }
}
