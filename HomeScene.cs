using Godot;

namespace EatEverything;

public partial class HomeScene : Node2D
{
	private const float Speed = 500.0f;
	
	private AnimatedSprite2D _knight;
	private CharacterBody2D _characterBody;

	public override void _Ready()
	{
		_characterBody = GetNode<CharacterBody2D>("CharacterBody2D");

		_knight = GetNode<AnimatedSprite2D>("CharacterBody2D/knight");
		_knight.Play("default");
	}
	
	public override void _Process(double delta)
	{
		// if (_knight == null) return;
		
		var velocity = Vector2.Zero;
		
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
		
		_characterBody.Position += velocity * Speed * (float)delta;		
		_characterBody.MoveAndSlide();
	}
}
