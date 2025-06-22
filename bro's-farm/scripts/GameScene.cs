using Godot;
using System;

public partial class GameScene : Node2D
{
	private PigFood pigFood;
	private Coop coop;
	private FarmingArea farmingArea;
	private AppleTree appleTree;
	private Well well;
	private BarnDoor barnDoor;

	private TextureButton buttonA;
	private TextureButton buttonB;

	private Joystick playerJoystick;

	public override void _Ready()
	{
		pigFood = GetNodeOrNull<PigFood>("pig_food");
		coop = GetNodeOrNull<Coop>("coop");
		farmingArea = GetNodeOrNull<FarmingArea>("FarmingArea");
		appleTree = GetNodeOrNull<AppleTree>("apple_tree/apple_tree");
		well = GetNodeOrNull<Well>("well");
		barnDoor = GetNodeOrNull<BarnDoor>("barn");
		buttonA = GetNodeOrNull<TextureButton>("CanvasLayer/buttonA");
		buttonB = GetNodeOrNull<TextureButton>("CanvasLayer/buttonB");
		playerJoystick = GetNodeOrNull<Joystick>("CanvasLayer/joystick");

		if (buttonA != null)
		{
			buttonA.Pressed += () =>
			{
				pigFood?.OnPlayerInteract(); // Sudah benar
				barnDoor?.TryEnterBarn();

				if (farmingArea?.IsPlayerNearby == true && farmingArea.CurrentPlot != null)
				{
					if (!farmingArea.CurrentPlot.IsPlanted)
						farmingArea.PlantIfNearby();
				}
			};
		}

		if (buttonB != null)
		{
			buttonB.Pressed += () =>
			{
				if (appleTree?.IsPlayerNearby == true && appleTree.CurrentApples > 0)
					appleTree.OnPlayerPressB();
				else if (well?.IsPlayerNearby == true && !well.BucketVisible)
					well.OnPlayerPressB();
				else if (coop?.IsPlayerNearby == true && coop.EggReady && coop.CurrentEggIndex < coop.MaxEggIndex)
					coop.OnPlayerPressB();
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
					Input.ActionPress("ui_up");
				else if (joystickDirection.Y > 0.5f)
					Input.ActionPress("ui_down");

				if (joystickDirection.X < -0.5f)
					Input.ActionPress("ui_left");
				else if (joystickDirection.X > 0.5f)
					Input.ActionPress("ui_right");
			}
		}
	}
}
