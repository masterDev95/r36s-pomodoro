using Godot;

public partial class Main : Control
{
	private const int DefaultDurationSeconds = 0;
	private const int DurationAdjustmentSeconds = 5;
	private const double TimerActionButtonCooldownMs = 150.0;
	private const double TurboTimerButtonCooldownMs = 50.0;

	private int _durationSeconds = DefaultDurationSeconds;
	private double _remainingSeconds;
	private double _timerActionButtonCooldownMs = 0;
	private double _turboTimerButtonCooldownMs = 0;

	private bool _isRunning = false;

	private TimerCircle _timerCircle;
	private AudioStreamPlayer _endSound;

	public override void _Ready()
	{
		_timerCircle = GetNode<TimerCircle>("Layout/TimerCircle");
		_endSound = GetNode<AudioStreamPlayer>("EndSound");

		ResetTimer();
	}

	public override void _Process(double delta)
	{
		UpdateCooldowns(delta);
		HandleDurationInputs();
		UpdateTimer(delta);
	}

	private void UpdateCooldowns(double delta)
	{
		_timerActionButtonCooldownMs = Mathf.Max(0.0, _timerActionButtonCooldownMs - delta * 1000.0);
		_turboTimerButtonCooldownMs = Mathf.Max(0.0, _turboTimerButtonCooldownMs - delta * 1000.0);
	}

	private void HandleDurationInputs()
	{
		if (Input.IsActionJustPressed("timer_toggle") && _remainingSeconds > 0.0)
		{
			_isRunning = !_isRunning;
		}

		// Only allow duration adjustments when the timer is not running
		if (_isRunning)
			return;

		if (Input.IsActionJustPressed("timer_reset"))
		{
			ResetTimer();
		}

		if (_timerActionButtonCooldownMs <= 0.0)
		{
			if (Input.IsActionPressed("timer_increase"))
			{
				AdjustDuration(DurationAdjustmentSeconds);
				_timerActionButtonCooldownMs = TimerActionButtonCooldownMs;
			}

			if (Input.IsActionPressed("timer_decrease"))
			{
				AdjustDuration(-DurationAdjustmentSeconds);
				_timerActionButtonCooldownMs = TimerActionButtonCooldownMs;
			}
		}

		if (_turboTimerButtonCooldownMs <= 0.0)
		{
			if (Input.IsActionPressed("turbo_increase"))
			{
				AdjustDuration(DurationAdjustmentSeconds);
				_turboTimerButtonCooldownMs = TurboTimerButtonCooldownMs;
			}

			if (Input.IsActionPressed("turbo_decrease"))
			{
				AdjustDuration(-DurationAdjustmentSeconds);
				_turboTimerButtonCooldownMs = TurboTimerButtonCooldownMs;
			}
		}
	}

	private void UpdateTimer(double delta)
	{
		if (!_isRunning)
			return;

		_remainingSeconds -= delta;

		if (_remainingSeconds <= 0.0)
		{
			_remainingSeconds = 0.0;
			_isRunning = false;
			if (_endSound.Stream != null)
				_endSound.Play();
		}

		UpdateDisplay();
	}

	private void AdjustDuration(int changeSeconds)
	{
		_durationSeconds = Mathf.Max(_durationSeconds + changeSeconds, 0);
		ResetTimer();
	}

	private void ResetTimer()
	{
		_isRunning = false;
		_remainingSeconds = _durationSeconds;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		float progress = _durationSeconds > 0 ? (float)(_remainingSeconds / _durationSeconds) : 0.0f;
		int displayedSeconds = (int)Mathf.Ceil(_remainingSeconds);

		_timerCircle.SetProgress(progress);
		_timerCircle.SetTime(displayedSeconds);
	}

}
