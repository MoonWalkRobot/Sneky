using Godot;
using System;

public class Snake : Node2D
{
	private Head head;
	public override void _Ready()
	{
		head = GetNode<Head>("Head");
	}

	public void SetInvincibility(float duration) 
	{
		head.BecomeInvincible(duration);
	}

	public void PlayAnimation(string animation)
	{
		head.PlayAnimation(animation);
	}

	//TODO: Invicibility frames.
	//TODO: prothèse dmg
	public void TakeDamage()
	{
		GetParent<MainGame>().TakeDamage();
	}

}
