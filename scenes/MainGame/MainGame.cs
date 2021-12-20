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
    private Timer fishTimer;
    public int FoodLevel = 0;
    public int HP = 5;
    public const int FoodLevelUp = 3;
    public int EatenFood = 0;
    public const int BombIncrease = 8;

    public override void _Ready()
    {
        Node2D fishes = new Node2D();
        fishes.Name = "Fishes";
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
        snake.GetNode<Head>("Head").Connect(nameof(Head.BorderHit),this,nameof(TakeDamage));
        AddChild(fishes);
        snake.Position = new Vector2(400, 400);
        fishTimer = GetNode<Timer>("FishTimer");
        fishTimer.Connect("timeout", this, nameof(onFishTimerTimeout));
        rng.Randomize();
        CreateFood();
        CreateBomb();
        CreateFauna(Fauna.FaunaType.clapfish);
        CreateFauna(Fauna.FaunaType.crab);
        CreateFauna(Fauna.FaunaType.eyes);
        fishTimer.Start(15);
    }

    public void TakeDamage()
    {
        HP--;
        snake.SetInvincibility(3);
        snake.PlayAnimation("Damage");
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
        food.GetNode<AnimationPlayer>("AnimationPlayer").Play("Spawn");
    }

    private async void CreateAltFood()
    {
        Food food = foodScene.Instance<Food>();
        food.Type = Food.Octopus.alt;
        mapElements.AddChild(food);
        food.Connect(nameof(Food.Dead), this, nameof(_OnFoodDead));
        food.Position = await findSpawnLocation();
        food.GetNode<AnimationPlayer>("AnimationPlayer").Play("Spawn");
    }

    private async void CreateBomb()
    {
        Bomb bomb = bombScene.Instance<Bomb>();
        mapElements.AddChild(bomb);
        bomb.Connect(nameof(Bomb.Dead), this, nameof(_OnBombDead));
        Connect(nameof(BombMove), bomb, nameof(Bomb.Die));
        bomb.Position = await findSpawnLocation();
        bomb.GetNode<AnimatedSprite>("AnimatedSprite").Play("Apparition");
    }

    private void CreateFauna(Fauna.FaunaType type)
    {
        Fauna fauna = faunaScene.Instance<Fauna>();
        fauna.Type = type;
        if (type == Fauna.FaunaType.crab)
        {
            fauna.Position = new Vector2(rng.Randf() * 1000 + 400, rng.Randf() * 700 + 100);
            mapElements.AddChild(fauna);
        }
        else if (type == Fauna.FaunaType.clapfish || type == Fauna.FaunaType.clownfish)
        {
            GetNode<Node2D>("Fishes").AddChild(fauna);
            fauna.SetupFish();
        }
        else if (type == Fauna.FaunaType.eyes)
        {
            fauna.Position = new Vector2(rng.Randf() * 200, rng.Randf() * 800);
            if (rng.Randf() >= 0.5)
            {
                fauna.Position = new Vector2(fauna.Position.x + 1400, fauna.Position.y);
            }
            mapElements.AddChild(fauna);
        }
    }

    private void _OnFoodDead(bool alt, bool reverse)
    {
        bool create = true;

        if (rng.Randf() >= 0.01 && !(alt || reverse))
        {
            CreateAltFood();
        }
        if (alt)
        {
            snake.GetNode<Head>("Head").AddBody(alt);
            HP++;
            EmitSignal(nameof(HealthUpdated), HP);
            create = false;
        }
        else if (reverse)
        {
            snake.GetNode<Head>("Head").Reverse(true);
            create = false;
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

        if (create)
        {
            CreateFood();
        }

        snake.PlayAnimation("Bite");
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

    private void onFishTimerTimeout()
    {
        float rnd = rng.Randf();
        CreateFauna((rnd >= 0.5 ? Fauna.FaunaType.clownfish : Fauna.FaunaType.clapfish));
        GD.Print("New Fish");
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
