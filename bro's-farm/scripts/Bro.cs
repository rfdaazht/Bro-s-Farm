using Godot;
using System;

public partial class Bro : CharacterBody2D
{
	[Export] public float Speed = 77f;

	private AnimatedSprite2D _anim;
	private string _lastDirection = "down"; // Default menghadap bawah

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("Bro_movement");
		PlayStandbyAnimation(_lastDirection); // Animasi awal saat game mulai
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;

		// Cegah gerakan diagonal dengan prioritas urutan: atas, bawah, kiri, kanan
		if (Input.IsActionPressed("ui_up"))
		{
			direction = Vector2.Up;
			_lastDirection = "up";
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			direction = Vector2.Down;
			_lastDirection = "down";
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			direction = Vector2.Left;
			_lastDirection = "left";
		}
		else if (Input.IsActionPressed("ui_right"))
		{
			direction = Vector2.Right;
			_lastDirection = "right";
		}

		// Gerakan & animasi
		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
			MoveAndSlide();
			PlayMoveAnimation(_lastDirection);
		}
		else
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			PlayStandbyAnimation(_lastDirection);
		}
	}

	private void PlayMoveAnimation(string dir)
	{
		switch (dir)
		{
			case "up":
				_anim.Play("idle_back");
				break;
			case "down":
				_anim.Play("idle_front");
				break;
			case "left":
				_anim.Play("idle_left");
				break;
			case "right":
				_anim.Play("idle_right");
				break;
		}
	}

	private void PlayStandbyAnimation(string dir)
	{
		switch (dir)
		{
			case "up":
				_anim.Play("sb_back");
				break;
			case "down":
				_anim.Play("sb_front");
				break;
			case "left":
				_anim.Play("sb_left");
				break;
			case "right":
				_anim.Play("sb_right");
				break;
		}
	}
}
