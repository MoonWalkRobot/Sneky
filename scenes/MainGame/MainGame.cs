using Godot;
using System.Threading.Tasks;

public class MainGame : Node2D
{
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private PackedScene foodScene = ResourceLoader.Load<PackedScene>("res://scenes/Food/Food.tscn");
    private PackedScene bombScene = ResourceLoader.Load<PackedScene>("res://scenes/Bomb/Bomb.tscn");
    private PackedScene faunaScene = ResourceLoader.Load<PackedScene>("res://scenes/Fauna/Fauna.tscn");
    private Globals globals;
    private Snake snake;
    private Bomb bomb;
    private YSort mapElements;
    public int FoodLevel = 0;
    public int HP = 1;
    public const int FoodLevelUp = 3;

    public override void _Ready()
    {
        mapElements = GetNode<YSort>("MapElements");
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

    private async void CreateFood()
    {
        Food food = foodScene.Instance<Food>();
        mapElements.AddChild(food);
        food.Connect(nameof(Food.Dead), this, nameof(_OnFoodDead));
        food.Position = await findSpawnLocation();
    }

    private async void CreateBomb()
    {
        Bomb bomb = bombScene.Instance<Bomb>();
        mapElements.AddChild(bomb);
        bomb.Connect(nameof(Bomb.Dead), this, nameof(_OnBombDead));
        bomb.Position = await findSpawnLocation();
    }

    private void CreateFauna()
    {
        Fauna fauna = faunaScene.Instance<Fauna>();
        fauna.Position = new Vector2(rng.Randf() * 500 + 100, rng.Randf() * 500 + 100); //TODO: FIX Generation.
        mapElements.AddChild(fauna);
    }

    private void _OnFoodDead(bool alt)
    {
        if (alt)
        {
            snake.GetNode<Head>("Head").AddBody(alt);
            HP++;
        }
        else
        {
            FoodLevel++;
            if (FoodLevel == FoodLevelUp)
            {
                FoodLevel = 0;
                snake.GetNode<Head>("Head").AddBody(alt);
            }
        }
        CreateFood();
    }

    private void _OnBombDead()
    {
        CreateBomb();
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
