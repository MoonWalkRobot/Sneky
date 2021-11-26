using Godot;
using System;

public class Bomb : Node2D
{
	[Signal] public delegate void Dead();

	public override void _Ready()
	{
		
	}

	public void Die() {
		EmitSignal(nameof(Dead));
		QueueFree();
	}
}
