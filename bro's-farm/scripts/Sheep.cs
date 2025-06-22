using Godot; // <<< Tambahkan ini
using System; // Jika System digunakan

public partial class Sheep : AnimalBase // Inherit from AnimalBase
{
	[Export] public float MinSheepSpeed = 15f; // New specific export for Sheep
	[Export] public float MaxSheepSpeed = 30f; // New specific export for Sheep

	public override void _Ready()
	{
		base._Ready();
		// No need to set BaseMoveSpeed directly, as Sheep uses a range
	}

	protected override void StartWalking()
	{
		ChooseNewDirection();
		// Override StartWalking to use a random speed within the sheep's range
		currentSpeed = (float)(random.NextDouble() * (MaxSheepSpeed - MinSheepSpeed) + MinSheepSpeed);
	}

	// Sheeps do not react to the player in this logic, so no OnPlayerEntered override needed.
}
