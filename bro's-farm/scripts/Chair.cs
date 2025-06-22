using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class Chair : Sprite2D
{
	private List<string> milkAnimations = new() { "milk_0", "milk_1", "milk_2", "milk_3", "milk_4", "milk_5", "milk_6" };
	private int currentIndex = 0;
	private const int MAX_MILK_INDEX = 6;

	private Area2D playerTrigger;
	private Area2D pickupArea;
	private Sprite2D fillButtonHint;
	private AnimatedSprite2D milkSprite;

	private bool playerNearby = false;
	private bool playerInPickupArea = false;
	private bool milkFull = false;
	private bool cooldownActive = false;

	private Tween currentFillHintTween;
	private Vector2 fillHintStartPos;

	private CancellationTokenSource cancelCooldown;

	public override void _Ready()
	{
		playerTrigger = GetNodeOrNull<Area2D>("Area2D");
		fillButtonHint = GetNodeOrNull<Sprite2D>("Area2D/ButtonHint");
		milkSprite = GetNodeOrNull<AnimatedSprite2D>("Milk");
		pickupArea = milkSprite?.GetNodeOrNull<Area2D>("PickupArea");

		if (playerTrigger == null || fillButtonHint == null || milkSprite == null || pickupArea == null)
			return;

		fillButtonHint.Visible = false;
		milkSprite.Play(milkAnimations[currentIndex]);
		milkSprite.Visible = false;

		fillHintStartPos = fillButtonHint.Position;

		playerTrigger.BodyEntered += OnBodyEntered;
		playerTrigger.BodyExited += OnBodyExited;
		pickupArea.BodyEntered += OnPickupAreaEntered;
		pickupArea.BodyExited += OnPickupAreaExited;

		StartHintAnimation(fillButtonHint, ref currentFillHintTween, fillHintStartPos);
		HideAllHints();
		cooldownActive = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = true;

			if (cooldownActive)
			{
				HideAllHints();
				milkSprite.Visible = false;
				return;
			}

			milkSprite.Visible = true;
			milkSprite.Play(milkAnimations[currentIndex]);

			if (milkFull)
				HideHint(fillButtonHint);
			else
				ShowHint(fillButtonHint);
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = false;
			playerInPickupArea = false;

			if (milkFull && !cooldownActive)
			{
				HideHint(fillButtonHint);
			}
			else
			{
				HideAllHints();
			}

			if (!milkFull && !cooldownActive)
			{
				milkSprite.Visible = false;
			}
		}
	}

	private void OnPickupAreaEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerInPickupArea = true;

			if (milkFull && !cooldownActive)
			{
				PerformMilkPickup();
			}
		}
	}

	private void OnPickupAreaExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerInPickupArea = false;
		}
	}

	public void OnPlayerFillMilk()
	{
		if (!playerNearby || milkFull || cooldownActive)
			return;

		if (currentIndex < MAX_MILK_INDEX)
		{
			currentIndex++;
			milkSprite.Play(milkAnimations[currentIndex]);

			if (currentIndex == MAX_MILK_INDEX)
			{
				milkFull = true;
				HideHint(fillButtonHint);
			}
			else
			{
				if (playerNearby)
					ShowHint(fillButtonHint);
				else
					HideHint(fillButtonHint);
			}
		}
	}

	private void PerformMilkPickup()
	{
		cooldownActive = true;
		cancelCooldown?.Cancel();
		cancelCooldown = new CancellationTokenSource();

		milkSprite.Visible = false;
		currentIndex = 0;
		milkFull = false;
		HideAllHints();

		_ = StartCooldownRoutine(cancelCooldown.Token);
	}

	private async Task StartCooldownRoutine(CancellationToken token)
	{
		try
		{
			await Task.Delay(30000, token);
			if (token.IsCancellationRequested)
				return;
		}
		catch (TaskCanceledException) { }
		finally
		{
			cooldownActive = false;

			if (playerNearby)
			{
				ShowHint(fillButtonHint);
				milkSprite.Visible = true;
				milkSprite.Play(milkAnimations[currentIndex]);
			}
		}
	}

	private void StartHintAnimation(Sprite2D hintSprite, ref Tween hintTween, Vector2 startPos)
	{
		if (hintTween != null && hintTween.IsRunning())
			return;
		if (hintSprite == null)
			return;

		hintTween = GetTree().CreateTween();
		hintTween.SetLoops();
		hintTween.TweenProperty(hintSprite, "position:y", startPos.Y - 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		hintTween.TweenProperty(hintSprite, "position:y", startPos.Y + 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}

	private void StopHintAnimation(Sprite2D hintSprite, ref Tween hintTween, Vector2 startPos)
	{
		if (hintTween != null && hintTween.IsRunning())
		{
			hintTween.Kill();
			hintTween = null;
		}
		if (hintSprite != null)
			hintSprite.Position = startPos;
	}

	private void ShowHint(Sprite2D hintSprite)
	{
		if (hintSprite != null && !hintSprite.Visible)
			hintSprite.Visible = true;
	}

	private void HideHint(Sprite2D hintSprite)
	{
		if (hintSprite != null && hintSprite.Visible)
			hintSprite.Visible = false;
	}

	private void HideAllHints()
	{
		HideHint(fillButtonHint);
	}

	public bool IsPlayerNearby()
	{
		return playerNearby;
	}
}
