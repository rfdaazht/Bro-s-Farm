using Godot;
using System.Threading.Tasks;

public partial class Plot : Area2D
{
	[Export] public string SeedType = "cabbage";

	public bool IsPlanted { get; private set; } = false;
	public bool PlayerNearby { get; private set; } = false;

	private Sprite2D highlight;
	private AnimatedSprite2D seed;
	private Sprite2D buttonHint;
	private Vector2 hintStartPos;
	private Tween hintTween;

	public override void _Ready()
	{
		highlight = GetNode<Sprite2D>("Highlight");
		seed = GetNode<AnimatedSprite2D>("Seed");
		buttonHint = GetNode<Sprite2D>("ButtonHint");

		highlight.Visible = false;
		seed.Visible = false;
		buttonHint.Visible = false;

		hintStartPos = buttonHint.Position;
		StartHintAnimation();

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Bro")
		{
			PlayerNearby = true;

			if (IsPlanted && seed.Animation == "seed_2")
			{
				Harvest();
				if (PlayerNearby && !IsPlanted)
					ShowButtonHint();
				else
					HideButtonHint();
			}
			else if (!IsPlanted)
			{
				highlight.Visible = true;
				ShowButtonHint();
			}
			else if (IsPlanted && (seed.Animation == "seed_0" || seed.Animation == "seed_1"))
			{
				highlight.Visible = false;
				HideButtonHint();
			}

			GetParent<FarmingArea>().NotifyPlotEnter(this);
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Bro")
		{
			PlayerNearby = false;
			highlight.Visible = false;
			HideButtonHint();

			GetParent<FarmingArea>().NotifyPlotExit(this);
		}
	}

	public async void Plant()
	{
		if (IsPlanted) return;

		IsPlanted = true;
		highlight.Visible = false;
		HideButtonHint();

		seed.Visible = true;
		seed.Play("seed_0");

		await Task.Delay(15000);
		if (seed.Animation == "seed_0")
		{
			seed.Play("seed_1");
		}

		await Task.Delay(15000);
		if (seed.Animation == "seed_1")
		{
			seed.Play("seed_2");
		}
	}

	private void Harvest()
	{
		IsPlanted = false;
		seed.Visible = false;

		if (PlayerNearby)
		{
			highlight.Visible = true;
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
