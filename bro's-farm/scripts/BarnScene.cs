using Godot;
using System;

public partial class BarnScene : Node2D
{
	private Fence fence2;
	private Fence fence3;
	private Fence fence4;
	private Fence fence5;
	private Chair chairInstance;
	private Chair chair1Instance;
	private Door doorInstance;
	
	private TextureButton buttonA;
	private TextureButton buttonB;
	
	private Joystick playerJoystick;

	public override void _Ready()
	{
		fence2 = GetNodeOrNull<Fence>("fence2");
		fence3 = GetNodeOrNull<Fence>("fence3");
		fence4 = GetNodeOrNull<Fence>("fence4");
		fence5 = GetNodeOrNull<Fence>("fence5");

		chairInstance = GetNodeOrNull<Chair>("chair");
		chair1Instance = GetNodeOrNull<Chair>("chair1");

		doorInstance = GetNodeOrNull<Door>("door");

		buttonA = GetNodeOrNull<TextureButton>("CanvasLayer/buttonA");
		buttonB = GetNodeOrNull<TextureButton>("CanvasLayer/buttonB");

		playerJoystick = GetNodeOrNull<Joystick>("CanvasLayer/joystick");

		if (fence2 == null) GD.PrintErr("Node fence2 tidak ditemukan!");
		if (fence3 == null) GD.PrintErr("Node fence3 tidak ditemukan!");
		if (fence4 == null) GD.PrintErr("Node fence4 tidak ditemukan!");
		if (fence5 == null) GD.PrintErr("Node fence5 tidak ditemukan!");
		if (chairInstance == null) GD.PrintErr("Node Chair 'chair' tidak ditemukan! Pastikan NodePath benar.");
		if (chair1Instance == null) GD.PrintErr("Node Chair 'chair1' tidak ditemukan! Pastikan NodePath benar.");
		if (doorInstance == null) GD.PrintErr("Node Door tidak ditemukan! Pastikan NodePath benar.");
		if (buttonA == null) GD.PrintErr("Node buttonA tidak ditemukan di CanvasLayer!");
		if (buttonB == null) GD.PrintErr("Node buttonB tidak ditemukan di CanvasLayer!");
		if (playerJoystick == null) GD.PrintErr("Node joystick tidak ditemukan di CanvasLayer! Pastikan jalur NodePath benar.");


		if (buttonA != null)
		{
			buttonA.Pressed += () =>
			{
				if (fence2?.IsPlayerNearby() == true) fence2.OnPlayerInteract();
				else if (fence3?.IsPlayerNearby() == true) fence3.OnPlayerInteract();
				else if (fence4?.IsPlayerNearby() == true) fence4.OnPlayerInteract();
				else if (fence5?.IsPlayerNearby() == true) fence5.OnPlayerInteract();
			};
		}

		if (buttonB != null)
		{
			buttonB.Pressed += () =>
			{
				if (chairInstance?.IsPlayerNearby() == true)
				{
					chairInstance.OnPlayerFillMilk();
				}
				else if (chair1Instance?.IsPlayerNearby() == true)
				{
					chair1Instance.OnPlayerFillMilk();
				}
				else if (doorInstance?.IsPlayerNearby == true)
				{
					doorInstance.TryEnterScene();
				}
			};
		}
	}

	public override void _Process(double delta)
	{
		if (playerJoystick != null)
		{
			Vector2 joystickDirection = playerJoystick.Direction;

			Input.ActionRelease("ui_up");
			Input.ActionRelease("ui_down");
			Input.ActionRelease("ui_left");
			Input.ActionRelease("ui_right");

			if (joystickDirection.Length() > 0.1f)
			{
				if (joystickDirection.Y < -0.5f)
				{
					Input.ActionPress("ui_up");
				}
				else if (joystickDirection.Y > 0.5f)
				{
					Input.ActionPress("ui_down");
				}

				if (joystickDirection.X < -0.5f)
				{
					Input.ActionPress("ui_left");
				}
				else if (joystickDirection.X > 0.5f)
				{
					Input.ActionPress("ui_right");
				}
			}
		}
	}
}
