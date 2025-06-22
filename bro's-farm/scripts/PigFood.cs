using Godot;
using System.Collections.Generic;
using System.Threading; // <--- PASTIKAN BARIS INI ADA
using System.Threading.Tasks;

public partial class PigFood : FoodDispenserBase
{
	public override void _Ready()
	{
		foodSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatableBody2D/AnimatedSprite2D");
		
		base._Ready();
	}

	protected override void InitializeFoodAnimations()
	{
		foodAnimations = new() { "food_0", "food_1", "food_2", "food_3", "food_4" };
	}

	protected override void InitialSetup()
	{
		currentIndex = 0;
		foodStateReadyForInteraction = true;
		ShowButtonHint();
		
		StartFoodDepletionMechanism();
	}

	protected override async Task AutoDepletionRoutine(CancellationToken token)
	{
		try
		{
			while (currentIndex > 0)
			{
				await Task.Delay(GetDepletionInterval(), token);
				if (token.IsCancellationRequested) return;

				RemoveFoodLevel();

				if (currentIndex == 0)
				{
					OnFoodBecameEmpty();
				}
			}
		}
		catch (TaskCanceledException) { }
	}

	protected override int GetDepletionInterval()
	{
		return 15000;
	}

	protected override void OnFoodBecameEmpty()
	{
		base.OnFoodBecameEmpty();
	}

	protected override int GetEmptyFoodDelayDuration()
	{
		return 15000;
	}
	
	protected override void UpdateHintVisibility()
	{
		if (!playerNearby)
		{
			HideButtonHint();
			return;
		}

		if (currentIndex < foodAnimations.Count - 1)
		{
			ShowButtonHint();
		}
		else
		{
			HideButtonHint();
		}
	}
	
	protected override bool IsSpecialEmptyHintPersistent()
	{
		return false; 
	}
}
