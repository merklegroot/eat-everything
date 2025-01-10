using Godot;
using System;
using System.Collections.Generic;

namespace EatEverything;

public partial class HomeScene : Node2D
{
	private const float Gravity = 8.0f;
	private const float Speed = 500.0f;
	
	private AnimatedSprite2D _knight;
	private CharacterBody2D _characterBody;

	private Vector2 _originalPosition;

	private TileMapLayer _tileMap;	

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

		_tileMap = GetNode<TileMapLayer>("GameTileMapLayer");
		var tiles = TileUtil.GetAllTiles(_tileMap);

		var collisionParent = TileUtil.GenerateCollisionShapesForTiles(_tileMap, tiles);
		AddChild(collisionParent);
	}

	private bool _wasJumpReleased = true;
	private float _jumpPower = 0;

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.R))
		{
			Reset();
			return;
		}

		var velocity = _characterBody.Velocity;

		// Handle horizontal movement
		float horizontalInput = 0;
		if (Input.IsKeyPressed(Key.A)) 
		{
			horizontalInput -= 1;
			_knight.FlipH = true;
		}
		if (Input.IsKeyPressed(Key.D)) 
		{
			horizontalInput += 1;
			_knight.FlipH = false;
		}
		
		
		// Handle jump
		if (Input.IsKeyPressed(Key.Space))
		{
			if (_wasJumpReleased && _characterBody.IsOnFloor())
			{
				GD.Print("Jump!");
				_jumpPower = 500;
			}
			
			_wasJumpReleased = false;
		}
		else
		{
			if (!_wasJumpReleased)
			{
				GD.Print("Jump released");
			}
			_wasJumpReleased = true;
		}

		if (_jumpPower > 0)
		{
			//const float maxJumpToConsume = 20;
			//var jumpToConsume = _jumpPower >= maxJumpToConsume ? maxJumpToConsume : _jumpPower;
			var jumpToConsume = _jumpPower / 10.0f;
			if (jumpToConsume < 10) jumpToConsume = _jumpPower;

			if (!_wasJumpReleased)
			{
				_jumpPower -= jumpToConsume;
			}
			else
			{
				_jumpPower -= 4 * jumpToConsume;;
			}
			
			if (_jumpPower < 0) _jumpPower = 0;
			
			velocity.Y -= jumpToConsume;
		}

		// Set horizontal velocity
		velocity.X = horizontalInput * Speed;
		
		// Apply gravity
		velocity.Y += Gravity;
		
		// Set the velocity and move
		_characterBody.Velocity = velocity;
		_characterBody.MoveAndSlide();
	}

	private void Reset()
	{
		_characterBody.Position = new Vector2(_originalPosition.X, _originalPosition.Y);
		_wasJumpReleased = true;
		_jumpPower = 0;
	}

	private List<TileData> GetAllTiles()
	{
		var tiles = new List<TileData>();
		
		// Get the used rect of the tilemap (area containing tiles)
		var usedRect = _tileMap.GetUsedRect();
		
		// Iterate through all cells in the rect
		for (int x = usedRect.Position.X; x < usedRect.Position.X + usedRect.Size.X; x++)
		{
			for (int y = usedRect.Position.Y; y < usedRect.Position.Y + usedRect.Size.Y; y++)
			{
				var pos = new Vector2I(x, y);
				
				// Check if there's a tile at this position
				if (_tileMap.GetCellSourceId(pos) != -1)  // 0 is the layer number
				{
					tiles.Add(new TileData
					{
						Position = pos,
						TileId = _tileMap.GetCellSourceId(pos),
						AtlasCoords = _tileMap.GetCellAtlasCoords(pos)
					});
				}
			}
		}

		return tiles;
	}
}
