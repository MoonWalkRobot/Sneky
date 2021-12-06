using Godot;
using System;

public class HealthBar : HBoxContainer
{
    private PackedScene healthBarItemScene = ResourceLoader.Load<PackedScene>("res://scenes/MainGame/HealthBarItem.tscn");

    public void UpdateHealth(int health)
    {
        int childCount = GetChildCount();
        int toAdd = health - childCount;

        if (toAdd > 0)
        {
            for (int i = 0; i < toAdd; i++)
            {
                AddChild(healthBarItemScene.Instance());
            }
        }
        else if (toAdd < 0)
        {
            Godot.Collections.Array children = GetChildren();
            toAdd = -toAdd;
            for (int i = 0; i < toAdd; i++)
            {
                Node child = (Node)children[i];
                child.QueueFree();
            }
        }
    }
}
