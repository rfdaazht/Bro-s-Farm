using Godot;
using System;

public partial class Joystick : Area2D
{
	[Export] public float Radius = 30.0f;
	[Export] public Texture2D BaseTexture;
	[Export] public Texture2D HandleTexture;

	private Sprite2D joystickSprite;
	private Vector2 joystickBasePosition;
	private bool isDragging = false;

	public Vector2 Direction { get; private set; } = Vector2.Zero;

	public override void _Ready()
	{
		joystickSprite = GetNodeOrNull<Sprite2D>("Sprite2D");

		if (joystickSprite == null)
		{
			GD.PrintErr("❌ Sprite2D joystick tidak ditemukan! Pastikan node Sprite2D bernama 'Sprite2D' adalah anak dari Joystick.");
			SetProcessInput(false);
			return;
		}

		joystickBasePosition = Position;

		if (HandleTexture != null)
		{
			joystickSprite.Texture = HandleTexture;
		}

		SetProcessInput(true);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Index == 0)
			{
				if (touchEvent.Pressed)
				{
					Vector2 localPos = ToLocal(touchEvent.Position);
					if (localPos.Length() <= Radius)
					{
						isDragging = true;
						GD.Print("Joystick ditekan.");
					}
				}
				else
				{
					if (isDragging)
					{
						isDragging = false;
						joystickSprite.Position = Vector2.Zero;
						Direction = Vector2.Zero;
						GD.Print("Joystick dilepas. Arah: " + Direction);
					}
				}
			}
		}
		else if (@event is InputEventScreenDrag dragEvent)
		{
			if (isDragging)
			{
				Vector2 localPosition = ToLocal(dragEvent.Position);
				if (localPosition.Length() > Radius)
				{
					localPosition = localPosition.Normalized() * Radius;
				}
				joystickSprite.Position = localPosition;
				Direction = localPosition.Normalized();
				GD.Print("Joystick diseret. Arah: " + Direction);
			}
		}
		else if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseEvent.Pressed)
				{
					Vector2 localPos = ToLocal(mouseEvent.Position);
					if (localPos.Length() <= Radius)
					{
						isDragging = true;
						GD.Print("Joystick diklik.");
					}
				}
				else
				{
					if (isDragging)
					{
						isDragging = false;
						joystickSprite.Position = Vector2.Zero;
						Direction = Vector2.Zero;
						GD.Print("Joystick dilepas. Arah: " + Direction);
					}
				}
			}
		}
		else if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			if (isDragging)
			{
				Vector2 localPosition = ToLocal(mouseMotionEvent.Position);
				if (localPosition.Length() > Radius)
				{
					localPosition = localPosition.Normalized() * Radius;
				}
				joystickSprite.Position = localPosition;
				Direction = localPosition.Normalized();
				GD.Print("Joystick diseret (mouse). Arah: " + Direction);
			}
		}
	}
}
