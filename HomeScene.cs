using Godot;
using System;
using System.Collections.Generic;

namespace EatEverything;

public partial class HomeScene : Node2D
{
	private const float Speed = 500.0f;
	
	private AnimatedSprite2D _knight;
	private CharacterBody2D _characterBody;

	private Vector2 _originalPosition;

	private TileMapLayer _tileMap;
	
	// You can customize this class based on what tile data you need
	private class TileData
	{
		public Vector2I Position { get; set; }
		public int TileId { get; set; }
		public Vector2I AtlasCoords { get; set; }
	}

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

		_tileMap = GetNode<TileMapLayer>("TileMapLayer");
		var tiles = GetAllTiles();

		// ShowTilePositions(tiles);
		CreateCollisionShapesForTiles(tiles);
	}

	private void ShowTilePositions(List<TileData> tiles)
	{
		foreach (var tile in tiles)
		{
			GD.Print($"Tile at {tile.Position}, ID: {tile.TileId}, Atlas: {tile.AtlasCoords}");
		}
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

		// Set horizontal velocity
		velocity.X = horizontalInput * Speed;

		// Apply gravity
		velocity.Y += 8.0f;

		// Handle jump
		if (Input.IsKeyPressed(Key.Space))
		{
			if (_wasJumpReleased && _characterBody.IsOnFloor())
			{
				_jumpStartTime = DateTime.UtcNow;
			}
			_wasJumpReleased = false;
		}
		else
		{
			_wasJumpReleased = true;
			_jumpStartTime = null;
		}

		if (_jumpStartTime.HasValue && !_wasJumpReleased)
		{
			var jumpElapsedTime = DateTime.UtcNow - _jumpStartTime;
			if (jumpElapsedTime >= TimeSpan.FromMilliseconds(250))
			{
				_jumpStartTime = null;
			}
			velocity.Y -= 24;
		}

		// Set the velocity and move
		_characterBody.Velocity = velocity;
		_characterBody.MoveAndSlide();
	}

	private void Reset()
	{
		_characterBody.Position = new Vector2(_originalPosition.X, _originalPosition.Y);
		_wasJumpReleased = true;
		_jumpStartTime = null;
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

	private void CreateCollisionShapesForTiles(List<TileData> tiles)
	{
		// Base tile size is 16x16
		var tileSize = new Vector2(16, 16);
		// Account for the TileMap's scale of 4 AND scale by 1.6 instead of 2
		var scaledTileSize = tileSize * 3.0f;
		
		GD.Print($"Scaled tile size: {scaledTileSize}");
		GD.Print($"TileMap position: {_tileMap.Position}");

		var collisionParent = new Node2D();
		AddChild(collisionParent);

		foreach (var tile in tiles)
		{
			var staticBody = new StaticBody2D();
			var collision = new CollisionShape2D();
			var shape = new RectangleShape2D();
			
			shape.Size = scaledTileSize;
			collision.Shape = shape;
			staticBody.AddChild(collision);

			// Get the tile's world position
			var tilePos = _tileMap.MapToLocal(tile.Position);
			GD.Print($"Tile at {tile.Position}, World pos: {tilePos}");
			
			// Scale the local position by 4 to match TileMap's scale
			var scaledPos = tilePos * 4;
			// Add TileMap position and center offset
			staticBody.Position = _tileMap.Position + scaledPos + scaledTileSize / 5.0f;
			
			collisionParent.AddChild(staticBody);
		}
	}
}
