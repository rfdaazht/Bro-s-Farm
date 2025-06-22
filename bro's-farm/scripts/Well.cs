using Godot;
using System.Threading.Tasks;

public partial class Well : Node2D
{
	private Area2D wellArea;
	private Sprite2D buttonHint;
	private Sprite2D waterBucket;
	private Area2D waterBucketArea;

	private bool playerNearWell = false;
	private bool bucketVisible = false;

	private Vector2 hintStartPos;
	private Tween currentHintTween;

	public bool IsPlayerNearby => playerNearWell;
	public bool BucketVisible => bucketVisible;

	public override void _Ready()
	{
		wellArea = GetNode<Area2D>("Area2D");
		buttonHint = wellArea.GetNode<Sprite2D>("ButtonHint");
		waterBucket = GetNode<Sprite2D>("WaterBucket");
		waterBucketArea = waterBucket.GetNode<Area2D>("Area2D");

		buttonHint.Visible = false;
		waterBucket.Visible = false;

		hintStartPos = buttonHint.Position;
		StartHintAnimation();

		wellArea.BodyEntered += OnWellAreaEntered;
		wellArea.BodyExited += OnWellAreaExited;
		waterBucketArea.BodyEntered += OnWaterBucketEntered;
	}

	public void OnPlayerPressB()
	{
		if (playerNearWell && !bucketVisible)
		{
			waterBucket.Visible = true;
			bucketVisible = true;
			HideButtonHint();
		}
	}

	private void OnWellAreaEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearWell = true;
			if (!bucketVisible)
				ShowButtonHint();
		}
	}

	private void OnWellAreaExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearWell = false;
			HideButtonHint();
		}
	}

	private void OnWaterBucketEntered(Node2D body)
	{
		if (body.Name == "Bro" && bucketVisible)
		{
			waterBucket.Visible = false;
			bucketVisible = false;

			if (playerNearWell)
				ShowButtonHint();
		}
	}

	private void ShowButtonHint()
	{
		if (buttonHint == null) return;
		if (!buttonHint.Visible)
		{
			buttonHint.Visible = true;
		}
	}

	private void HideButtonHint()
	{
		if (buttonHint == null) return;
		if (buttonHint.Visible)
		{
			buttonHint.Visible = false;
		}
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

	private void StopHintAnimation()
	{
		if (currentHintTween != null && currentHintTween.IsRunning())
		{
			currentHintTween.Kill();
			currentHintTween = null;
		}

		buttonHint.Position = hintStartPos;
	}
}
