using Godot;
using System;

public class MainGame : Node2D
{
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private PackedScene foodScene = ResourceLoader.Load<PackedScene>("res://scenes/Food/Food.tscn");
    private PackedScene bombScene = ResourceLoader.Load<PackedScene>("res://scenes/Bomb/Bomb.tscn");
    private Snake snake;
    private Bomb bomb;
    public int foodLevel = 0;
    public const int foodLevelUp = 3;

    public override void _Ready()
    {
        snake = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Snake.tscn").Instance<Snake>();
        AddChild(snake);
        snake.Position = new Vector2(400, 400);
        rng.Randomize();
        CreateFood();
        CreateBomb();
    }

    private void CreateFood()
    {
        Food food = foodScene.Instance<Food>();
        food.Position = new Vector2(rng.Randf() * 500 + 100, rng.Randf() * 500 + 100); //TODO: FIX Generation.
        AddChild(food);
        food.Connect(nameof(Food.Dead), this, nameof(_OnFoodDead));
    }

    private void CreateBomb()
    {
        bomb = bombScene.Instance<Bomb>();
        bomb.Position = new Vector2(rng.Randf() * 500 + 100, rng.Randf() * 500 + 100); //TODO: FIX Generation.
        AddChild(bomb);
        bomb.Connect(nameof(Bomb.Dead), this, nameof(_OnBombDead));
    }

    private void _OnFoodDead()
    {
        foodLevel++;
        if (foodLevel == foodLevelUp)
        {
            foodLevel = 0;
            snake.GetNode<Head>("Head").AddBody();
            bomb.Die();
        }
        CreateFood();
    }

    private void _OnBombDead()
    {
        snake.TakeDamage();
        CreateBomb();
    }


}
