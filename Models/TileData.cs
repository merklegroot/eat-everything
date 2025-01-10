using Godot;

public record TileData
{
    public Vector2I Position { get; set; }
    public int TileId { get; set; }
    public Vector2I AtlasCoords { get; set; }
}