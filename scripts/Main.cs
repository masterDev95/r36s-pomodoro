using Godot;
using System;

public partial class Main : Control
{
	private const double TimerActionButtonCooldownMs = 150.0;

	private int _durationSeconds = 10;
	private double _remainingSeconds;
	private double _timerActionButtonCooldownMs = 0;

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
		if (_timerActionButtonCooldownMs > 0.0)
		{
			_timerActionButtonCooldownMs -= delta * 1000;
			if (_timerActionButtonCooldownMs < 0.0)
				_timerActionButtonCooldownMs = 0.0;
		}
		else
		{
			if (Input.IsActionPressed("timer_increase"))
			{
				ChangeDuration(5);
				_timerActionButtonCooldownMs = TimerActionButtonCooldownMs;
			}
			
			if (Input.IsActionPressed("timer_decrease"))
			{
				ChangeDuration(-5);
				_timerActionButtonCooldownMs = TimerActionButtonCooldownMs;
			}
		}

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

	public override void _UnhandledInput(InputEvent e)
	{
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
		// Clamp the duration to be between 10 seconds and 1 hour (3600 seconds)
		_durationSeconds = Mathf.Clamp(_durationSeconds + changeSeconds, 10, 3600);

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
