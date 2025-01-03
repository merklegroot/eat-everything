using Godot;
using System;

public partial class HomeScene : Node2D
{
	private AnimatedSprite2D _knight;
	private const float SPEED = 400.0f; // Pixels per second

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get reference to the knight node
		_knight = GetNode<AnimatedSprite2D>("knight");		
		_knight.Play("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_knight == null) return;

		Vector2 velocity = Vector2.Zero;

		// Get input
		if (Input.IsKeyPressed(Key.W)) velocity.Y -= 1;
		if (Input.IsKeyPressed(Key.S)) velocity.Y += 1;
		if (Input.IsKeyPressed(Key.A)) 
		{
			velocity.X -= 1;
			_knight.FlipH = true;
		}
		if (Input.IsKeyPressed(Key.D)) 
		{
			velocity.X += 1;
			_knight.FlipH = false;
		}

		// Normalize velocity to prevent faster diagonal movement
		velocity = velocity.Normalized();

		// Move the knight
		_knight.Position += velocity * SPEED * (float)delta;
	}
}
