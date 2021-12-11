using Godot;
using System.Threading.Tasks;

public class MainGame : Control
{
    [Signal] public delegate void HealthUpdated(int health);
    [Signal] public delegate void FoodLevelUpdated(float foodLevel, float foodLevelMax);
    [Signal] public delegate void BombMove();
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private PackedScene foodScene = ResourceLoader.Load<PackedScene>("res://scenes/Food/Food.tscn");
    private PackedScene bombScene = ResourceLoader.Load<PackedScene>("res://scenes/Bomb/Bomb.tscn");
    private PackedScene faunaScene = ResourceLoader.Load<PackedScene>("res://scenes/Fauna/Fauna.tscn");
    private PackedScene gameOverScene = ResourceLoader.Load<PackedScene>("res://scenes/MainGame/GameOver.tscn");
    private Globals globals;
    private Snake snake;
    private Bomb bomb;
    private YSort mapElements;
    private FoodBar foodBar;
    private HealthBar healthBar;
    public int FoodLevel = 0;
    public int HP = 1;
    public const int FoodLevelUp = 3;
    public int EatenFood = 0;
    public const int BombIncrease = 8;

    public override void _Ready()
    {
        mapElements = GetNode<YSort>("MapElements");
        foodBar = GetNode<FoodBar>("UI/VBoxContainer/FoodBar");
        healthBar = GetNode<HealthBar>("UI/VBoxContainer/HealthBar");
        Connect(nameof(FoodLevelUpdated), foodBar, nameof(FoodBar.UpdateFoodLevel));
        Connect(nameof(HealthUpdated), healthBar, nameof(HealthBar.UpdateHealth));
        EmitSignal(nameof(HealthUpdated), HP);
        snake = ResourceLoader.Load<PackedScene>("res://scenes/Snake/Snake.tscn").Instance<Snake>();
        globals = GetNode<Globals>("/root/Globals");
        if (globals.EnableShaders)
        {
            AddChild(ResourceLoader.Load<PackedScene>("res://scenes/Shader/Shader.tscn").Instance());
        }
        AddChild(snake);
        snake.Position = new Vector2(400, 400);
        rng.Randomize();
        CreateFood();
        CreateBomb();
    }

    public void TakeDamage()
    {
        HP--;
        if (HP <= 0)
        {
            gameOver();
        }
        else
        {
            EmitSignal("HealthUpdated", HP);
        }
    }

    private async void CreateFood()
    {
        Food food = foodScene.Instance<Food>();
        mapElements.AddChild(food);
        food.Connect(nameof(Food.Dead), this, nameof(_OnFoodDead));
        food.Position = await findSpawnLocation();
    }

    private async void CreateAltFood()
    {
        Food food = foodScene.Instance<Food>();
        food.Type = Food.Octopus.alt;
        mapElements.AddChild(food);
        food.Connect(nameof(Food.Dead), this, nameof(_OnFoodDead));
        food.Position = await findSpawnLocation();
    }

    private async void CreateBomb()
    {
        Bomb bomb = bombScene.Instance<Bomb>();
        mapElements.AddChild(bomb);
        bomb.Connect(nameof(Bomb.Dead), this, nameof(_OnBombDead));
        Connect(nameof(BombMove), bomb, nameof(Bomb.Die));
        bomb.Position = await findSpawnLocation();
    }

    private void CreateFauna()
    {
        Fauna fauna = faunaScene.Instance<Fauna>();
        fauna.Position = new Vector2(rng.Randf() * 500 + 100, rng.Randf() * 500 + 100); //TODO: FIX Generation.
        mapElements.AddChild(fauna);
    }

    private void _OnFoodDead(bool alt, bool reverse)
    {
        if (rng.Randf() >= 0.15 && !(alt || reverse))
        {
            CreateAltFood();
        }
        if (alt)
        {
            snake.GetNode<Head>("Head").AddBody(alt);
            HP++;
            EmitSignal(nameof(HealthUpdated), HP);
            return;
        }
        else if (reverse)
        {
            snake.GetNode<Head>("Head").ReverseRotation();
            return;
        }
        else
        {
            FoodLevel++;
            if (FoodLevel == FoodLevelUp)
            {
                FoodLevel = 0;
                snake.GetNode<Head>("Head").AddBody(alt);
                EmitSignal("BombMove");
            }
            EmitSignal(nameof(FoodLevelUpdated), FoodLevel, FoodLevelUp - 1);
        }
        EatenFood++;
        if (EatenFood % BombIncrease == 0)
        {
            CreateBomb();
        }
        CreateFood();
    }

    private void _OnBombDead()
    {
        CreateBomb();
    }

    private void gameOver()
    {
        GameOver gameOver = gameOverScene.Instance<GameOver>();
        gameOver.Connect(nameof(GameOver.Retry), this, nameof(onGameOverRetry));
        gameOver.Connect(nameof(GameOver.MainMenu), this, nameof(onGameOverMainMenu));
        GetTree().Paused = true;
        AddChild(gameOver);
    }

    private void onGameOverRetry()
    {
        GetTree().Paused = false;
        GetParent().AddChild(ResourceLoader.Load<PackedScene>("res://scenes/MainGame/MainGame.tscn").Instance());
        QueueFree();
    }

    private void onGameOverMainMenu()
    {
        GetTree().Paused = false;
        GetParent().AddChild(ResourceLoader.Load<PackedScene>("res://scenes/MainMenu/MainMenu.tscn").Instance());
        QueueFree();
    }

    // LOCATION FINDER
    private async Task<bool> isPositionSpawnable(Vector2 v)
    {
        bool res = true;
        Node2D locationFinderHelper = ResourceLoader.Load<PackedScene>("res://scenes/MainGame/LocationFinderHelper.tscn").Instance<Node2D>();
        AddChild(locationFinderHelper);
        locationFinderHelper.Position = v;
        await ToSignal(GetTree(), "physics_frame");
        Godot.Collections.Array a = locationFinderHelper.GetNode<Area2D>("Area2D").GetOverlappingAreas();
        foreach (Area2D area in a)
        {
            Node2D node = area.GetParent<Node2D>();
            if (node is Body || node is Head || node is Food || node is Bomb)
            {
                res = false;
            }
        }
        locationFinderHelper.QueueFree();
        return res;
    }

    private Vector2 mapTopLeft = new Vector2(315, 85);
    private Vector2 mapBottomRight = new Vector2(1290, 820);
    private Vector2 generateLocation()
    {
        return new Vector2(rng.Randf() * (mapBottomRight.x - mapTopLeft.x) + mapTopLeft.x, rng.Randf() * (mapBottomRight.y - mapTopLeft.y) + mapTopLeft.y);
    }

    private async Task<Vector2> findSpawnLocation()
    {
        Vector2 res;
        res = generateLocation();
        while (!await isPositionSpawnable(res))
        {
            res = generateLocation();
        }
        return res;
    }

    // /LOCATION FINDER
}
