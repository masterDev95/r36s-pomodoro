using Godot;

public partial class Main : Control
{
	private const double TimerActionButtonCooldownMs = 150.0;
	private const double TurboTimerButtonCooldownMs = 50.0;

	private int _durationSeconds = 10;
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

		_remainingSeconds = _durationSeconds;

		UpdateDisplay();
	}

	public override void _Process(double delta)
	{
		// Handle the cooldown for the action buttons (increase/decrease)
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
		
		if (Input.IsActionPressed("turbo_increase"))
		{
			ChangeDuration(5);
			_turboTimerButtonCooldownMs = TurboTimerButtonCooldownMs;
		}

		if (Input.IsActionPressed("turbo_decrease"))
		{
			ChangeDuration(-5);
			_turboTimerButtonCooldownMs = TurboTimerButtonCooldownMs;
		}

		// Update the timer if it's running
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

	private void ChangeDuration(int changeSeconds)
	{
		_durationSeconds = Mathf.Max(_durationSeconds + changeSeconds, 0);

		// The changing of the duration resets the timer
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
		float progress = (float)(_remainingSeconds / _durationSeconds);
		int displayedSeconds = (int)Mathf.Ceil(_remainingSeconds);

		_timerCircle.SetProgress(progress);
		_timerCircle.SetTime(displayedSeconds);
	}
}
