using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class Fence : FoodDispenserBase
{
	[Export]
	public NodePath CowTarget;

	private Node2D cow;
	private bool firstFeedDone = false;

	public override void _Ready()
	{
		foodSprite = GetNodeOrNull<AnimatedSprite2D>("Grass");

		base._Ready(); 

		if (CowTarget != null)
			cow = GetNodeOrNull<Node2D>(CowTarget);
	}

	protected override void InitializeFoodAnimations()
	{
		foodAnimations = new() { "grass_0", "grass_1", "grass_2", "grass_3" };
	}

	protected override void InitialSetup()
	{
		currentIndex = 0;
		foodStateReadyForInteraction = true;
		ShowButtonHint();
		
		StartFoodDepletionMechanism();
	}

	public override void OnPlayerInteract()
	{
		if (!firstFeedDone)
		{
			firstFeedDone = true;
		}

		base.OnPlayerInteract();

		if (currentIndex == foodAnimations.Count - 1)
		{
			SetCowEatingState(true);
			foodStateReadyForInteraction = false;
			HideButtonHint();
		}
	}

	protected override int GetDepletionInterval()
	{
		return 10000;
	}

	protected override void OnFoodBecameEmpty()
	{
		SetCowEatingState(false);
		base.OnFoodBecameEmpty();
	}

	protected override int GetEmptyFoodDelayDuration()
	{
		return 30000;
	}

	protected override bool IsSpecialEmptyHintPersistent()
	{
		return true;
	}

	private void SetCowEatingState(bool isEating)
	{
		if (cow == null) return;

		var cowAnim = cow.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (cowAnim == null) return;

		cowAnim.SpeedScale = isEating ? 2.0f : 1.0f;
	}
}
