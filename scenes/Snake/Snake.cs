using Godot;
using System;

public class Snake : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Head head;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // head = this.GetNode<Head>("Head");
        // head.Position = new Vector2(2,2);
        // Piece piece = (Piece)((PackedScene)ResourceLoader.Load("res://src/Pieces/" + pieceName + "/" + pieceName + ".tscn")).Instance();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
