using Godot;

public partial class FarmingArea : Node2D
{
	public Plot CurrentPlot { get; private set; }

	public override void _Ready() { }

	public void NotifyPlotEnter(Plot plot)
	{
		CurrentPlot = plot;
	}

	public void NotifyPlotExit(Plot plot)
	{
		if (CurrentPlot == plot)
		{
			CurrentPlot = null;
		}
	}

	public void PlantIfNearby()
	{
		if (CurrentPlot != null && CurrentPlot.PlayerNearby && !CurrentPlot.IsPlanted)
		{
			CurrentPlot.Plant();
		}
	}

	public bool IsPlayerNearby => CurrentPlot != null && CurrentPlot.PlayerNearby;
}
