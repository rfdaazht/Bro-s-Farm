using Godot; // <<< Tambahkan ini
using System; // Jika System digunakan

public partial class Chick : AnimalBase
{
	public override void _Ready()
	{
		base._Ready(); // Call base _Ready to initialize anim, playerDetector, etc.
		BaseMoveSpeed = 25f; // Set specific move speed for Chick
	}

	// Override OnPlayerEntered specific to Chick's behavior (fleeing)
	protected override void OnPlayerEntered(Node body)
	{
		if (body.Name == "Bro")
		{
			player = (Node2D)body;
			Vector2 diff = GlobalPosition - player.GlobalPosition;
			fleeDirection = diff.Normalized();
			fleeTimer = fleeDuration;
			isWalking = true; // Ensure they start moving to flee
		}
	}
}
