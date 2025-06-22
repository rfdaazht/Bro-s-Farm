using Godot;
using System.Threading.Tasks;

public partial class AppleTree : Area2D
{
	[Export] public int MaxApples = 5;
	private int currentApples = 0;

	private AnimatedSprite2D treeSprite;
	private Sprite2D buttonHint;

	private bool playerNearby = false;
	private Vector2 hintStartPos;
	private Tween hintTween;

	public bool IsPlayerNearby => playerNearby;
	public int CurrentApples => currentApples;

	public override void _Ready()
	{
		treeSprite = GetNode<AnimatedSprite2D>("TreeSprite");
		buttonHint = GetNode<Sprite2D>("ButtonHint");

		if (treeSprite == null) GD.PrintErr("Node TreeSprite tidak ditemukan di AppleTree!");
		if (buttonHint == null) GD.PrintErr("Node ButtonHint tidak ditemukan di AppleTree!");

		buttonHint.Visible = false;
		hintStartPos = buttonHint.Position;

		UpdateAppleVisual();
		StartHintAnimation();
		_ = StartGrowingApples();

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Bro" && currentApples > 0)
		{
			playerNearby = true;
			buttonHint.Visible = true;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			playerNearby = false;
			buttonHint.Visible = false;
		}
	}

	public void OnPlayerPressB()
	{
		if (playerNearby && currentApples > 0)
		{
			currentApples--;
			UpdateAppleVisual();
			_ = GrowBackAfterDelay();
		}
	}

	private async Task GrowBackAfterDelay()
	{
		await Task.Delay(10000);
		if (currentApples < MaxApples)
		{
			currentApples++;
			UpdateAppleVisual();
		}
	}

	private async Task StartGrowingApples()
	{
		while (currentApples < MaxApples)
		{
			await Task.Delay(10000);
			currentApples++;
			UpdateAppleVisual();
		}
	}

	private void UpdateAppleVisual()
	{
		treeSprite.Play($"apple_{currentApples}");

		if (playerNearby)
			buttonHint.Visible = currentApples > 0;
	}

	private void StartHintAnimation()
	{
		if (hintTween != null && hintTween.IsRunning())
		{
			hintTween.Kill();
			hintTween = null;
		}

		hintTween = GetTree().CreateTween();
		hintTween.SetLoops();
		hintTween.TweenProperty(buttonHint, "position:y", hintStartPos.Y - 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		hintTween.TweenProperty(buttonHint, "position:y", hintStartPos.Y + 2, 0.5f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}
}
