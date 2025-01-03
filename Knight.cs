using Godot;

public partial class Knight : AnimatedSprite2D
{
	public override void _Ready()
	{
		GD.Print("Knight script is running");
		
		if (SpriteFrames == null)
		{
			GD.PrintErr("No SpriteFrames assigned!");
			return;
		}

		GD.Print("Animation names: " + string.Join(", ", SpriteFrames.GetAnimationNames()));
		Play("default", 1.0f, true);
	}
} 
