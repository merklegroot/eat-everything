using Godot;
using System;

public partial class HomeScene : Node2D
{
	private AnimatedSprite2D _knight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get reference to the knight node
		_knight = GetNode<AnimatedSprite2D>("knight");
		
		// Start the animation
		if (_knight != null)
		{
			_knight.Play("default");
		}
		else
		{
			GD.PrintErr("Could not find knight node!");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
