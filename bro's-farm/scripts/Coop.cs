using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class Coop : Sprite2D
{
	private Area2D playerTrigger;
	private Sprite2D buttonHint;
	private AnimatedSprite2D eggRack;
	private Area2D pickupArea;

	private int currentEggIndex = 0;
	private const int maxEggIndex = 8;

	private bool playerNearby = false;
	private bool eggReady = false;

	private Vector2 hintStartPos;
	private Tween currentHintTween;

	private CancellationTokenSource eggTimerCancel;

	public bool IsPlayerNearby => playerNearby;
	public bool EggReady => eggReady;
	public int CurrentEggIndex => currentEggIndex;
	public int MaxEggIndex => maxEggIndex;

	public override void _Ready()
	{
		playerTrigger = GetNodeOrNull<Area2D>("Area2D");
		buttonHint = GetNodeOrNull<Sprite2D>("Area2D/ButtonHint");
		eggRack = GetNodeOrNull<AnimatedSprite2D>("EggRack");
		pickupArea = eggRack?.GetNodeOrNull<Area2D>("PickupArea");

		if (eggRack != null)
			eggRack.Visible = false;

		if (playerTrigger != null)
		{
			playerTrigger.BodyEntered += OnBodyEntered;
			playerTrigger.BodyExited += OnBodyExited;
		}

		if (pickupArea != null)
		{
			pickupArea.BodyEntered += OnPickupEntered;
			pickupArea.Monitoring = true;
			pickupArea.Monitorable = true;
		}

		if (buttonHint != null)
		{
			buttonHint.Visible = false;
			hintStartPos = buttonHint.Position;
			StartHintAnimation();
		}

		StartEggCycleDelay();
	}

	private async void StartEggCycleDelay()
	{
		eggTimerCancel?.Cancel();
		eggTimerCancel = new CancellationTokenSource();

		eggReady = false;

		if (!playerNearby && eggRack != null)
			eggRack.Visible = false;

		HideButtonHint();

		try
		{
			await Task.Delay(15000, eggTimerCancel.Token);
		}
		catch (TaskCanceledException)
		{
			return;
		}

		eggReady = true;

		if (currentEggIndex < maxEggIndex)
		{
			ShowButtonHint();
			if (playerNearby && eggRack != null && currentEggIndex == 0)
			{
				eggRack.Visible = true;
				eggRack.Play("egg_0");
			}
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = true;

			if (eggReady && currentEggIndex < maxEggIndex)
			{
				ShowButtonHint();
				if (eggRack != null)
				{
					eggRack.Visible = true;
					eggRack.Play($"egg_{currentEggIndex}");
				}
			}
			else if (currentEggIndex == maxEggIndex && eggRack != null)
			{
				eggRack.Visible = true;
				eggRack.Play($"egg_{maxEggIndex}");
			}
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = false;

			if (eggRack != null && currentEggIndex < maxEggIndex)
				eggRack.Visible = false;

			if (!eggReady || currentEggIndex == maxEggIndex)
				HideButtonHint();
		}
	}

	public void OnPlayerPressB()
	{
		if (!playerNearby || eggRack == null || !eggReady || currentEggIndex == maxEggIndex)
			return;

		if (currentEggIndex < maxEggIndex)
		{
			currentEggIndex++;
			eggRack.Visible = true;
			eggRack.Play($"egg_{currentEggIndex}");

			if (currentEggIndex == maxEggIndex)
			{
				eggReady = false;
				HideButtonHint();
			}
		}
	}

	private void OnPickupEntered(Node2D body)
	{
		if (body.Name == "Bro" && currentEggIndex == maxEggIndex && eggRack != null && eggRack.Visible)
		{
			eggRack.Visible = false;
			currentEggIndex = 0;

			StartEggCycleDelay();
			HideButtonHint();
		}
	}

	private void ShowButtonHint()
	{
		if (buttonHint == null) return;
		if (!buttonHint.Visible)
			buttonHint.Visible = true;
	}

	private void HideButtonHint()
	{
		if (buttonHint == null) return;
		if (buttonHint.Visible)
			buttonHint.Visible = false;
	}

	private void StartHintAnimation()
	{
		if (currentHintTween != null && currentHintTween.IsRunning())
			return;

		currentHintTween = GetTree().CreateTween();
		currentHintTween.SetLoops();

		currentHintTween.TweenProperty(
			buttonHint, "position:y",
			hintStartPos.Y - 2, 0.5f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		currentHintTween.TweenProperty(
			buttonHint, "position:y",
			hintStartPos.Y + 2, 0.5f
		).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}
}
