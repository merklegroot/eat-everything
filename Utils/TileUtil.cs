using System.Collections.Generic;
using Godot;

public static class TileUtil
{
	public static List<TileData> GetAllTiles(TileMapLayer tileMap)
	{
		var tiles = new List<TileData>();
		
		// Get the used rect of the tilemap (area containing tiles)
		var usedRect = tileMap.GetUsedRect();
		
		// Iterate through all cells in the rect
		for (int x = usedRect.Position.X; x < usedRect.Position.X + usedRect.Size.X; x++)
		{
			for (int y = usedRect.Position.Y; y < usedRect.Position.Y + usedRect.Size.Y; y++)
			{
				var pos = new Vector2I(x, y);
				
				// Check if there's a tile at this position
				if (tileMap.GetCellSourceId(pos) != -1)  // 0 is the layer number
				{
					tiles.Add(new TileData
					{
						Position = pos,
						TileId = tileMap.GetCellSourceId(pos),
						AtlasCoords = tileMap.GetCellAtlasCoords(pos)
					});
				}
			}
		}

		return tiles;
	}

	public static Node2D GenerateCollisionShapesForTiles(TileMapLayer tileMap, List<TileData> tiles)
	{
		// Base tile size is 16x16
		var tileSize = new Vector2(16, 16);
		// Account for the TileMap's scale of 4 AND scale by 1.6 instead of 2
		var scaledTileSize = tileSize * 3.0f;
		
		GD.Print($"Scaled tile size: {scaledTileSize}");
		// GD.Print($"TileMap position: {_tileMap.Position}");

		var collisionParent = new Node2D();		

		foreach (var tile in tiles)
		{
			var staticBody = new StaticBody2D();
			var collision = new CollisionShape2D();
			var shape = new RectangleShape2D();
			
			shape.Size = scaledTileSize;
			collision.Shape = shape;
			staticBody.AddChild(collision);

			// Get the tile's world position
			var tilePos = tileMap.MapToLocal(tile.Position);
			GD.Print($"Tile at {tile.Position}, World pos: {tilePos}");
			
			// Scale the local position by 4 to match TileMap's scale
			var scaledPos = tilePos * 4;
			// Add TileMap position and center offset
			staticBody.Position = tileMap.Position + scaledPos + scaledTileSize / 5.0f;
			
			collisionParent.AddChild(staticBody);
		}

		return collisionParent;
	}
}
