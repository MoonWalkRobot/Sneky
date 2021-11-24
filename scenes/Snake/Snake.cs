using Godot;
using System;

public class Snake : Node2D
{
    private Head head;
    public int hp = 1;
    public override void _Ready()
    {
        head = GetNode<Head>("Head");
    }

    //TODO: Invicibility frames.
    //TODO: prothèse dmg
    public void TakeDamage()
    {
        hp--;
        //TODO: Play the damage animation
        if (hp <= 0) {
            //TODO: Loose the game
            GD.Print("dead");
        }
    }

}
