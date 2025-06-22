using Godot;
using System;

public partial class Bro : CharacterBody2D
{
	[Export] public float Speed = 77f;

	private AnimatedSprite2D _anim;
	private string _lastDirection;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("Bro_movement");
		
		_lastDirection = "down";
		
		PlayStandbyAnimation(_lastDirection);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDirection = Vector2.Zero;

		if (Input.IsActionPressed("ui_up"))
		{
			inputDirection.Y -= 1;
		}
		if (Input.IsActionPressed("ui_down"))
		{
			inputDirection.Y += 1;
		}
		if (Input.IsActionPressed("ui_left"))
		{
			inputDirection.X -= 1;
		}
		if (Input.IsActionPressed("ui_right"))
		{
			inputDirection.X += 1;
		}

		if (inputDirection.Length() > 1.0f)
		{
			inputDirection = inputDirection.Normalized();
		}

		Velocity = inputDirection * Speed;
		MoveAndSlide();

		if (inputDirection != Vector2.Zero)
		{
			if (inputDirection.X != 0 && inputDirection.Y != 0)
			{
				if (inputDirection.X > 0)
					_lastDirection = "right";
				else
					_lastDirection = "left";
			}
			else
			{
				if (Mathf.Abs(inputDirection.X) > Mathf.Abs(inputDirection.Y))
				{
					if (inputDirection.X > 0)
						_lastDirection = "right";
					else
						_lastDirection = "left";
				}
				else
				{
					if (inputDirection.Y > 0)
						_lastDirection = "down";
					else
						_lastDirection = "up";
				}
			}
			PlayMoveAnimation(_lastDirection);
		}
		else
		{
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
