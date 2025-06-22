using Godot;
using System;

public partial class Door : Sprite2D
{
	private Area2D triggerArea;
	private Sprite2D buttonHint;
	private bool playerNearby = false;
	private Tween hintTween;
	private Vector2 hintStartPos;

	[Export]
	public string TargetScenePath { get; set; } = "res://scenes/game_scene.tscn";

	public override void _Ready()
	{
		triggerArea = GetNodeOrNull<Area2D>("Area2D");
		buttonHint = GetNodeOrNull<Sprite2D>("Area2D/ButtonHint");

		if (buttonHint != null)
		{
			buttonHint.Visible = false;
			hintStartPos = buttonHint.Position;
		}

		if (triggerArea != null)
		{
			triggerArea.BodyEntered += OnBodyEntered;
			triggerArea.BodyExited += OnBodyExited;
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = true;
			ShowButtonHint();
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = false;
			HideButtonHint();
		}
	}

	private void ShowButtonHint()
	{
		if (buttonHint == null) return;
		if (!buttonHint.Visible)
		{
			buttonHint.Visible = true;
			StartHintAnimation();
		}
	}

	private void HideButtonHint()
	{
		if (buttonHint == null) return;
		if (buttonHint.Visible)
		{
			buttonHint.Visible = false;
			StopHintAnimation();
		}
	}

	private void StartHintAnimation()
	{
		StopHintAnimation();
		hintTween = GetTree().CreateTween();
		hintTween.SetLoops();
		hintTween.TweenProperty(buttonHint, "position:y", hintStartPos.Y - 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		hintTween.TweenProperty(buttonHint, "position:y", hintStartPos.Y + 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}

	private void StopHintAnimation()
	{
		if (hintTween != null && hintTween.IsRunning())
		{
			hintTween.Kill();
			hintTween = null;
		}
		if (buttonHint != null)
			buttonHint.Position = hintStartPos;
	}

	public void TryEnterScene()
	{
		if (playerNearby)
		{
			GetTree().ChangeSceneToFile(TargetScenePath);
		}
	}

	public bool IsPlayerNearby => playerNearby;
}
