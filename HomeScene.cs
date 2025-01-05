using Godot;
using System;

namespace EatEverything;

public partial class HomeScene : Node2D
{
	private const float Speed = 500.0f;
	
	private AnimatedSprite2D _knight;
	private CharacterBody2D _characterBody;

	private Vector2 _originalPosition;


	private static class Animations
	{
		public const string Horizontal = "horizontal_animation";
		public const string Jump = "jump_animation";
	}

	public override void _Ready()
	{
		_characterBody = GetNode<CharacterBody2D>("CharacterBody2D");

		_knight = GetNode<AnimatedSprite2D>("CharacterBody2D/knight");
		_knight.Play(Animations.Horizontal);

		_originalPosition = new Vector2(_characterBody.Position.X, _characterBody.Position.Y);
	}

	private bool _wasJumpReleased = true;
	private DateTime? _jumpStartTime;
	
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.R))
		{
			Reset();
			return;
		}

		var velocity = Vector2.Zero;

		if (_jumpStartTime.HasValue)
		{
			var jumpElapsedTime = DateTime.UtcNow - _jumpStartTime;;
			if(jumpElapsedTime >= TimeSpan.FromMilliseconds(250))
			{
				_jumpStartTime = null;
			}
			
			velocity.Y -= 2;
		}

		velocity.Y += 1.0f;
		
		if (Input.IsKeyPressed(Key.Space))
		{
			if (_wasJumpReleased)
			{
				_jumpStartTime = DateTime.UtcNow;
			}

			_wasJumpReleased = false;
		}
		else
		{
			_wasJumpReleased = true;
		}

		// if (Input.IsKeyPressed(Key.W)) velocity.Y -= 1;
		// if (Input.IsKeyPressed(Key.S)) velocity.Y += 1;
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
		// velocity = velocity.Normalized();
		
		_characterBody.Position += velocity * Speed * (float)delta;		
		_characterBody.MoveAndSlide();
	}

	private void Reset()
	{
		_characterBody.Position = new Vector2(_originalPosition.X, _originalPosition.Y);
		_wasJumpReleased = true;
		_jumpStartTime = null;
	}
}
