using Godot;
using System;

public partial class Pig : AnimalBase
{
	[Export] public float MinPigSpeed = 10f;
	[Export] public float MaxPigSpeed = 25f;

	public override void _Ready()
	{
		base._Ready();
	}

	protected override void StartWalking()
	{
		ChooseNewDirection();
		currentSpeed = (float)(random.NextDouble() * (MaxPigSpeed - MinPigSpeed) + MinPigSpeed);
	}
}
