using Godot;
using System;

public partial class Main : Control
{
	private int _durationSeconds = 25 * 60;
	private double _remainingSeconds;

	private bool _isRunning = false;

	private TimerCircle _timerCircle;
	private Button _startPauseButton;
	private AudioStreamPlayer _endSound;

	public override void _Ready()
	{
		_timerCircle = GetNode<TimerCircle>("Layout/TimerCircle");
		_startPauseButton = GetNode<Button>("Layout/Controls/StartPauseButton");
		_endSound = GetNode<AudioStreamPlayer>("EndSound");

		Button decreaseButton = GetNode<Button>("Layout/Controls/DecreaseButton");
		Button increaseButton = GetNode<Button>("Layout/Controls/IncreaseButton");
		Button resetButton = GetNode<Button>("Layout/Controls/ResetButton");

		_startPauseButton.Pressed += OnStartPauseButtonPressed;
		decreaseButton.Pressed += OnDecreaseButtonPressed;
		increaseButton.Pressed += OnIncreaseButtonPressed;
		resetButton.Pressed += OnResetButtonPressed;

		_remainingSeconds = _durationSeconds;

		UpdateDisplay();

		_startPauseButton.GrabFocus();
	}

	public override void _Process(double delta)
	{
		if (!_isRunning)
			return;

		_remainingSeconds -= delta;

		if (_remainingSeconds <= 0.0)
		{
			_remainingSeconds = 0.0;
			_isRunning = false;

			GD.Print("Pomodoro finished!");

			_startPauseButton.Text = "START";

			if (_endSound.Stream != null)
				_endSound.Play();
		}

		UpdateDisplay();
	}

	private void OnStartPauseButtonPressed()
	{
		if (_isRunning)
		{
			// Pause the timer
			_isRunning = false;
			_startPauseButton.Text = "REPRENDRE";
		}
		else
		{
			// If the timer is at 0, reset it to the full duration
			if (_remainingSeconds <= 0.0)
				ResetTimer();

			_isRunning = true;
			_startPauseButton.Text = "PAUSE";
		}
	}

	private void OnIncreaseButtonPressed()
	{
		ChangeDuration(5 * 60);
	}

	private void OnDecreaseButtonPressed()
	{
		ChangeDuration(-5 * 60);
	}

	private void OnResetButtonPressed()
	{
		ResetTimer();
	}

	private void ChangeDuration(int changeSeconds)
	{
		// 120 minutes max
		_durationSeconds = Mathf.Clamp(_durationSeconds + changeSeconds, 60, 120 * 60);

		// The changing of the duration resets the timer
		ResetTimer();
	}

	private void ResetTimer()
	{
		_isRunning = false;
		_remainingSeconds = _durationSeconds;
		_startPauseButton.Text = "START";
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		int displayedSeconds = (int)Mathf.Ceil(_remainingSeconds);

		_timerCircle.SetTime(displayedSeconds);

		float progress = (float)(_remainingSeconds / _durationSeconds);

		_timerCircle.SetProgress(progress);
	}
}
