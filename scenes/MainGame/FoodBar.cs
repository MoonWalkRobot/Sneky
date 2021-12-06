using Godot;
using System;

public class FoodBar : TextureProgress
{
    public override void _Ready()
    {   
    }

    public void UpdateFoodLevel(float foodLevel, float foodMaxLevel)
    {
        Value = (foodLevel / foodMaxLevel) * 80f + 20f;
    }
}
