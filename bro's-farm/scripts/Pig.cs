using Godot; // <<< Tambahkan ini
using System; // Jika System digunakan

public partial class Pig : AnimalBase // Inherit from AnimalBase
{
	[Export] public float MinPigSpeed = 10f; // New specific export for Pig
	[Export] public float MaxPigSpeed = 25f; // New specific export for Pig

	public override void _Ready()
	{
		base._Ready();
		// No need to set BaseMoveSpeed directly, as Pig uses a range
	}

	protected override void StartWalking()
	{
		ChooseNewDirection();
		// Override StartWalking to use a random speed within the pig's range
		currentSpeed = (float)(random.NextDouble() * (MaxPigSpeed - MinPigSpeed) + MinPigSpeed);
	}

	// Pigs do not react to the player in this logic, so no OnPlayerEntered override needed.
	// The base OnPlayerEntered will do nothing if not overridden.
}
