using Godot;
using System;

public partial class Sheep : AnimalBase
{
	[Export] public float MinSheepSpeed = 15f;
	[Export] public float MaxSheepSpeed = 30f;

	public override void _Ready()
	{
		base._Ready();
	}

	protected override void StartWalking()
	{
		ChooseNewDirection();
		currentSpeed = (float)(random.NextDouble() * (MaxSheepSpeed - MinSheepSpeed) + MinSheepSpeed);
	}
}
