using Godot;
using System;

public partial class AnimalBase : CharacterBody2D
{
	[Export] public float BaseMoveSpeed = 20f;
	[Export] public float WalkDuration = 2f;
	[Export] public float IdleDuration = 1.5f;

	protected AnimatedSprite2D anim;
	protected Vector2 direction = Vector2.Zero;
	protected float stateTimer = 0f;
	protected bool isWalking = true;
	protected Random random = new Random();

	protected float currentSpeed;
	private float collisionCooldown = 0f;

	protected Area2D playerDetector;
	protected Node2D player;
	protected float fleeTimer = 0f;
	protected float fleeDuration = 1.2f;
	protected Vector2 fleeDirection = Vector2.Zero;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (HasNode("Area2D")) 
		{
			playerDetector = GetNode<Area2D>("Area2D");
			playerDetector.BodyEntered += OnPlayerEntered;
		}

		InitializeMovementState();
	}

	public override void _Process(double delta)
	{
		float d = (float)delta;
		stateTimer -= d;
		collisionCooldown -= d;

		if (fleeTimer > 0f)
		{
			fleeTimer -= d;
			Velocity = fleeDirection * BaseMoveSpeed * 1.5f;
			MoveAndSlide();
			PlayWalkAnimation(fleeDirection);
			return;
		}

		if (stateTimer <= 0f)
		{
			isWalking = !isWalking;
			stateTimer = isWalking
				? (float)(random.NextDouble() * WalkDuration + 0.5f)
				: (float)(random.NextDouble() * IdleDuration + 0.5f);

			if (isWalking)
				StartWalking();
			else
				direction = Vector2.Zero;
		}

		if (isWalking)
		{
			Velocity = direction.Normalized() * currentSpeed;
			MoveAndSlide();
			HandleCollisions();
			PlayWalkAnimation(direction);
		}
		else
		{
			Velocity = Vector2.Zero;
			PlayIdleAnimation();
		}
	}

	protected virtual void InitializeMovementState()
	{
		isWalking = random.Next(2) == 0;
		stateTimer = (float)(isWalking
			? random.NextDouble() * WalkDuration
			: random.NextDouble() * IdleDuration);

		if (isWalking)
			StartWalking();
		else
			direction = Vector2.Zero;
	}

	protected virtual void StartWalking()
	{
		ChooseNewDirection();
		currentSpeed = BaseMoveSpeed;
	}

	protected virtual void ChooseNewDirection()
	{
		Vector2[] directions = {
			Vector2.Up,
			Vector2.Down,
			Vector2.Left,
			Vector2.Right,
			(Vector2.Up + Vector2.Left).Normalized(),
			(Vector2.Up + Vector2.Right).Normalized(),
			(Vector2.Down + Vector2.Left).Normalized(),
			(Vector2.Down + Vector2.Right).Normalized()
		};

		direction = directions[random.Next(directions.Length)];
	}

	protected virtual void HandleCollisions()
	{
		if (collisionCooldown <= 0f && GetSlideCollisionCount() > 0)
		{
			if (random.NextDouble() < 0.5)
			{
				StartWalking();
			}
			else
			{
				isWalking = false;
				stateTimer = (float)(random.NextDouble() * IdleDuration + 0.5f);
				direction = Vector2.Zero;
			}
			collisionCooldown = 1f;
		}
	}

	protected void PlayWalkAnimation(Vector2 dir)
	{
		if (dir == Vector2.Zero)
			return;

		if (Mathf.Abs(dir.X) >= Mathf.Abs(dir.Y))
		{
			anim.Play(dir.X < 0 ? "idle_left" : "idle_right");
		}
		else
		{
			anim.Play(dir.Y < 0 ? "idle_back" : "idle_front");
		}
	}

	protected void PlayIdleAnimation()
	{
		string currentAnim = anim.Animation.ToString();

		if (currentAnim.StartsWith("idle_"))
		{
			switch (currentAnim)
			{
				case "idle_front": anim.Play("sb_front"); break;
				case "idle_back": anim.Play("sb_back"); break;
				case "idle_left": anim.Play("sb_left"); break;
				case "idle_right": anim.Play("sb_right"); break;
				default: anim.Play("sb_front"); break;
			}
		}
	}

	protected virtual void OnPlayerEntered(Node body)
	{
	}
}
