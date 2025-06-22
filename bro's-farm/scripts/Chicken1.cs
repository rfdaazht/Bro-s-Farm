using Godot; // <<< Tambahkan ini
using System; // Jika System digunakan

public partial class Chicken1 : AnimalBase
{
	public override void _Ready()
	{
		base._Ready();
		BaseMoveSpeed = 18f;
	}

	protected override void OnPlayerEntered(Node body)
	{
		if (body.Name == "Bro")
		{
			player = (Node2D)body;
			Vector2 diff = GlobalPosition - player.GlobalPosition;
			fleeDirection = diff.Normalized();
			fleeTimer = fleeDuration;
			isWalking = true;
		}
	}
}
