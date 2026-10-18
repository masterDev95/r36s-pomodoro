using Godot;
using System;

public partial class WheelsContainer : HBoxContainer
{
	public enum Wheel
	{
		Minute,
		Second
	}

	private const Wheel DefaultWheel = Wheel.Minute;
	private const int DurationAdjustmentSeconds = 5;

	private Wheel _activeWheel;

	// Minute wheel
	private Label _minutesTopLabel;
	private Label _minutesCenterLabel;
	private Label _minutesBottomLabel;
	private TextureRect _minutesChevronUp;
	private TextureRect _minutesChevronDown;

	// Second wheel
	private Label _secondsTopLabel;
	private Label _secondsCenterLabel;
	private Label _secondsBottomLabel;
	private TextureRect _secondsChevronUp;
	private TextureRect _secondsChevronDown;

	public Wheel ActiveWheel
	{
		get => _activeWheel;
		private set => _activeWheel = value;
	}

	public override void _Ready()
	{
		// Minute wheel
		_minutesTopLabel = GetNode<Label>("MinuteWheel/TopLabel");
		_minutesCenterLabel = GetNode<Label>("MinuteWheel/CenterLabel");
		_minutesBottomLabel = GetNode<Label>("MinuteWheel/BottomLabel");
		_minutesChevronUp = GetNode<TextureRect>("MinuteWheel/ChevronUp");
		_minutesChevronDown = GetNode<TextureRect>("MinuteWheel/ChevronDown");

		// Second wheel
		_secondsTopLabel = GetNode<Label>("SecondWheel/TopLabel");
		_secondsCenterLabel = GetNode<Label>("SecondWheel/CenterLabel");
		_secondsBottomLabel = GetNode<Label>("SecondWheel/BottomLabel");
		_secondsChevronUp = GetNode<TextureRect>("SecondWheel/ChevronUp");
		_secondsChevronDown = GetNode<TextureRect>("SecondWheel/ChevronDown");

		// Set the default active wheel
		SetActiveWheel(DefaultWheel);
	}

	public void SetActiveWheel(Wheel wheel)
	{
		ActiveWheel = wheel;

		// Update the visibility of chevrons based on the active wheel
		_minutesChevronUp.Visible = ActiveWheel == Wheel.Minute;
		_minutesChevronDown.Visible = ActiveWheel == Wheel.Minute;
		_secondsChevronUp.Visible = ActiveWheel == Wheel.Second;
		_secondsChevronDown.Visible = ActiveWheel == Wheel.Second;
	}

	public void UpdateWheels(int minutes, int seconds)
	{
		// Update minute wheel
		_minutesCenterLabel.Text = minutes.ToString("D2");
		_minutesTopLabel.Text = ((minutes + 1) % 60).ToString("D2");
		_minutesBottomLabel.Text = ((minutes - 1 + 60) % 60).ToString("D2");

		// Update second wheel
		_secondsCenterLabel.Text = seconds.ToString("D2");
		_secondsTopLabel.Text = ((seconds + 5) % 60).ToString("D2");
		_secondsBottomLabel.Text = ((seconds - 5 + 60) % 60).ToString("D2");
	}
}
