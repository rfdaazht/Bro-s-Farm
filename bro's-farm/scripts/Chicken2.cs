using Godot;
using System;

public partial class Chicken2 : AnimalBase
{
	public override void _Ready()
	{
		base._Ready();
		BaseMoveSpeed = 20f;
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
