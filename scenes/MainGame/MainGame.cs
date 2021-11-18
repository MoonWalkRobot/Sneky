using Godot;
using System;

public class MainGame : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    RandomNumberGenerator rng = new RandomNumberGenerator();
    PackedScene foodScene = ResourceLoader.Load<PackedScene>("res://scenes/Food/Food.tscn");



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Snake snake = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Snake.tscn").Instance<Snake>();
        AddChild(snake);
        snake.Position = new Vector2(400,400);
        rng.Randomize();
        CreateFood();
    }

    private void CreateFood() {
        Food food = foodScene.Instance<Food>();
        food.Position = new Vector2(rng.Randf() * 800, rng.Randf() * 800);
        AddChild(food);
        food.Connect(nameof(Food.Dead), this, nameof(_OnFoodDead));
    }

    private void _OnFoodDead() {
        CreateFood();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
