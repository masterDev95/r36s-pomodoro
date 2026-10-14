using Godot;
using System;

public partial class TimerCircle : Control
{
	// Progression : 1 = full circle, 0 = empty circle
	private float _progress = 1.0f;

	private Label _timeLabel;

	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("TimeLabel");
	}

	public override void _Draw()
	{
		// Center and radius of the circle
		Vector2 center = Size / 2.0f;
		float radius = Mathf.Min(Size.X, Size.Y) / 2.0f - 12.0f;

		// Circle thickness
		float thickness = 20.0f;

		// Draw the background circle
		DrawArc(
			center,
			radius,
			-Mathf.Pi / 2.0f,
			3.0f * Mathf.Pi / 2.0f,
			96,
			new Color("#272829"),
			thickness,
			true
		);

		// Draw the progress arc
		if (_progress > 0.0f)
		{
			float endAngle = -Mathf.Pi / 2.0f + Mathf.Tau * _progress;

			Vector2 startPoint = center + new Vector2(
				Mathf.Cos(-Mathf.Pi / 2.0f),
				Mathf.Sin(-Mathf.Pi / 2.0f)
			) * radius;

			Vector2 endPoint = center + new Vector2(
				Mathf.Cos(endAngle),
				Mathf.Sin(endAngle)
			) * radius;

			DrawCircle(startPoint, thickness / 2.0f, new Color("#283c59"));
			DrawCircle(endPoint, thickness / 2.0f, new Color("#283c59"));

			DrawArc(
				center,
				radius,
				-Mathf.Pi / 2.0f,
				endAngle,
				96,
				new Color("#283c59"),
				thickness,
				true
			);
		}
	}

	public void SetProgress(float progress)
	{
		_progress = Mathf.Clamp(progress, 0.0f, 1.0f);
		QueueRedraw();
	}

	public void SetTime(int totalSeconds)
	{
		totalSeconds = Mathf.Max(0, totalSeconds);

		int minutes = totalSeconds / 60;
		int seconds = totalSeconds % 60;

		_timeLabel.Text = $"{minutes:00}   :   {seconds:00}";
	}
}
