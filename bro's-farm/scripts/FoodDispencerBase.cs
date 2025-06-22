using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public abstract partial class FoodDispenserBase : Sprite2D
{
	protected List<string> foodAnimations = new();
	protected int currentIndex = 0;

	protected Area2D playerTrigger;
	protected Sprite2D buttonHint;
	protected AnimatedSprite2D foodSprite;

	protected bool playerNearby = false;
	protected bool foodStateReadyForInteraction = true;

	protected CancellationTokenSource cancelOperation;
	protected CancellationTokenSource cancelDelayAfterEmpty;

	protected Tween currentHintTween;
	protected Vector2 hintStartPos;

	public override void _Ready()
	{
		playerTrigger = GetNodeOrNull<Area2D>("Area2D"); // Pastikan ini ada
		buttonHint = GetNodeOrNull<Sprite2D>("Area2D/ButtonHint"); // Pastikan ini ada

		foodSprite = GetNodeOrNull<AnimatedSprite2D>("Grass");
		if (foodSprite == null)
		{
			foodSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatableBody2D/AnimatedSprite2D");
		}
		
		if (foodSprite == null)
		{
			GD.PrintErr($"FoodDispenserBase: AnimatedSprite2D not found on {Name}. Make sure it's named 'Grass' or 'AnimatableBody2D/AnimatedSprite2D' or override FoodSpritePath.");
			return;
		}

		InitializeFoodAnimations(); 
		if (foodAnimations.Count > 0)
		{
			foodSprite.Play(foodAnimations[currentIndex]);
		}

		if (playerTrigger != null && buttonHint != null) // Periksa keduanya
		{
			buttonHint.Visible = false;
			hintStartPos = buttonHint.Position;
			playerTrigger.BodyEntered += OnBodyEntered;
			playerTrigger.BodyExited += OnBodyExited;
		}
		else
		{
			GD.PrintErr($"FoodDispenserBase: Area2D or ButtonHint not found on {Name}. Cannot initialize player trigger or hint.");
		}

		InitialSetup();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("interact") && playerNearby)
		{
			OnPlayerInteract();
		}
	}

	protected abstract void InitializeFoodAnimations();

	protected virtual void InitialSetup()
	{
		if (currentIndex == 0)
		{
			ShowButtonHint();
			foodStateReadyForInteraction = true;
		}
		StartFoodDepletionMechanism();
	}

	// Perubahan di sini: 'public virtual'
	public virtual void OnPlayerInteract()
	{
		CancelAllOperations();

		if (CanAddFood())
		{
			AddFoodLevel();
			AfterFoodAdded();
		}
		else if (CanRemoveFood())
		{
			RemoveFoodLevel();
			AfterFoodRemoved();
		}

		StartFoodDepletionMechanism();
	}

	protected virtual bool CanAddFood()
	{
		return currentIndex < foodAnimations.Count - 1;
	}

	protected virtual bool CanRemoveFood()
	{
		return false;
	}

	protected virtual void AddFoodLevel()
	{
		currentIndex++;
		foodSprite.Play(foodAnimations[currentIndex]);
		foodStateReadyForInteraction = false;
		UpdateHintVisibility();
	}

	protected virtual void AfterFoodAdded()
	{
		
	}

	protected virtual void RemoveFoodLevel()
	{
		currentIndex--;
		foodSprite.Play(foodAnimations[currentIndex]);
		UpdateHintVisibility();
	}

	protected virtual void AfterFoodRemoved()
	{
		
	}

	protected virtual void StartFoodDepletionMechanism()
	{
		cancelOperation?.Cancel();
		cancelOperation = new CancellationTokenSource();
		_ = AutoDepletionRoutine(cancelOperation.Token);
	}

	protected virtual async Task AutoDepletionRoutine(CancellationToken token)
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

	protected virtual int GetDepletionInterval()
	{
		return 10000;
	}

	protected virtual void OnFoodBecameEmpty()
	{
		foodStateReadyForInteraction = false;
		HideButtonHint();
		StartDelayAfterEmpty();
	}

	protected virtual void StartDelayAfterEmpty()
	{
		cancelDelayAfterEmpty?.Cancel();
		cancelDelayAfterEmpty = new CancellationTokenSource();
		_ = BeginDelayAfterEmptyRoutine(cancelDelayAfterEmpty.Token);
	}

	protected virtual async Task BeginDelayAfterEmptyRoutine(CancellationToken token)
	{
		try
		{
			await Task.Delay(GetEmptyFoodDelayDuration(), token);
			if (token.IsCancellationRequested) return;

			if (currentIndex == 0)
			{
				foodStateReadyForInteraction = true;
				ShowButtonHint();
			}
		}
		catch (TaskCanceledException) { }
	}

	protected virtual int GetEmptyFoodDelayDuration()
	{
		return 30000;
	}
	
	protected void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = true;
			UpdateHintVisibility();
		}
	}

	protected void OnBodyExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = false;
			if (currentIndex == 0 && foodStateReadyForInteraction && IsSpecialEmptyHintPersistent())
			{
				return;
			}
			HideButtonHint();
		}
	}

	protected virtual bool IsSpecialEmptyHintPersistent()
	{
		return false; 
	}

	protected virtual void UpdateHintVisibility()
	{
		if (!playerNearby)
		{
			HideButtonHint();
			return;
		}

		if (foodStateReadyForInteraction || currentIndex < foodAnimations.Count - 1)
		{
			ShowButtonHint();
		}
		else
		{
			HideButtonHint();
		}
	}

	private void CancelAllOperations()
	{
		cancelOperation?.Cancel();
		cancelDelayAfterEmpty?.Cancel();
	}

	protected void StartHintAnimation()
	{
		StopHintAnimation();
		currentHintTween = GetTree().CreateTween();
		currentHintTween.SetLoops();
		currentHintTween.TweenProperty(buttonHint, "position:y", hintStartPos.Y - 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		currentHintTween.TweenProperty(buttonHint, "position:y", hintStartPos.Y + 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}

	protected void StopHintAnimation()
	{
		if (currentHintTween != null && currentHintTween.IsRunning())
		{
			currentHintTween.Kill();
			currentHintTween = null;
		}
		if (buttonHint != null)
			buttonHint.Position = hintStartPos;
	}

	// Perubahan di sini: Memastikan StartHintAnimation dipanggil
	protected void ShowButtonHint()
	{
		if (buttonHint != null && !buttonHint.Visible)
		{
			buttonHint.Visible = true;
			StartHintAnimation(); // <--- BARIS INI PENTING!
		}
	}

	protected void HideButtonHint()
	{
		if (buttonHint != null && buttonHint.Visible)
		{
			buttonHint.Visible = false;
			StopHintAnimation();
		}
	}

	public bool IsPlayerNearby()
	{
		return playerNearby;
	}
}
