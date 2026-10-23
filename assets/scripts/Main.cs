using Godot;
using static WheelsContainer;

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
	private TextureRect _playPauseIcon;
	private WheelsContainer _wheelsController;

	public override void _Ready()
	{
		_timerCircle = GetNode<TimerCircle>("MainLayout/TimerLayout/TimerCircle");
		_endSound = GetNode<AudioStreamPlayer>("EndSound");
		_wheelsController = GetNode<WheelsContainer>("MainLayout/WheelsContainer");
		_playPauseIcon = GetNode<TextureRect>("MainLayout/TimerLayout/PlayPauseContainer/PlayPauseIcon");

		_remainingSeconds = _durationSeconds;

		ResetTimer();
	}

	public override void _Process(double delta)
	{
		UpdateCooldowns(delta);
		HandleDurationInputs();
		UpdateTimer(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsEcho())
			return;

		if (@event.IsActionPressed("timer_toggle"))
		{
			SetRunningState(!_isRunning);
			GetViewport().SetInputAsHandled();
		}
		
		if (!_isRunning && @event.IsActionPressed("timer_reset"))
		{
			ResetTimer();
			GetViewport().SetInputAsHandled();
		}

		if (@event.IsActionPressed("ui_left"))
		{
			_wheelsController.SetActiveWheel(Wheel.Minute);
			GetViewport().SetInputAsHandled();
		}

		if (@event.IsActionPressed("ui_right"))
		{
			_wheelsController.SetActiveWheel(Wheel.Second);
			GetViewport().SetInputAsHandled();
		}
	}

	private void UpdateCooldowns(double delta)
	{
		_timerActionButtonCooldownMs = Mathf.Max(0.0, _timerActionButtonCooldownMs - delta * 1000.0);
		_turboTimerButtonCooldownMs = Mathf.Max(0.0, _turboTimerButtonCooldownMs - delta * 1000.0);
	}

	private void HandleDurationInputs()
	{
		// Only allow duration adjustments when the timer is not running
		if (_isRunning)
			return;

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

			if (Input.IsActionPressed("ui_up"))
			{
				if (_wheelsController.ActiveWheel == Wheel.Minute)
				{
					if (_durationSeconds < 59 * 60)
						AdjustDuration(60);
					else
						AdjustDuration(-59 * 60);
				}
				else
				{
					if (_durationSeconds % 60 < 60 - DurationAdjustmentSeconds)
						AdjustDuration(DurationAdjustmentSeconds);
					else
						AdjustDuration(-(60 - DurationAdjustmentSeconds));
				}

				_timerActionButtonCooldownMs = TimerActionButtonCooldownMs;
			}

			if (Input.IsActionPressed("ui_down"))
			{
				if (_wheelsController.ActiveWheel == Wheel.Minute)
				{
					if (_durationSeconds >= 60)
						AdjustDuration(-60);
					else		
						AdjustDuration(59 * 60);
				}
				else
				{
					if (_durationSeconds % 60 / DurationAdjustmentSeconds >= 1)
						AdjustDuration(-DurationAdjustmentSeconds);
					else
						AdjustDuration(60 - DurationAdjustmentSeconds);
				}

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

	private void SetRunningState(bool isRunning)
	{
		if (!_isRunning && _durationSeconds <= 0)
			return;

		_isRunning = isRunning;
		
		if (!_isRunning)
			_playPauseIcon.Texture = GD.Load<Texture2D>("res://assets/icons/play.svg");
		else
			_playPauseIcon.Texture = GD.Load<Texture2D>("res://assets/icons/pause.svg");
	}

	private void UpdateTimer(double delta)
	{
		if (!_isRunning)
			return;

		_remainingSeconds -= delta;

		if (_remainingSeconds <= 0.0)
		{
			_remainingSeconds = 0.0;
			_durationSeconds = 0;
			SetRunningState(false);
			if (_endSound.Stream != null)
				_endSound.Play();
		}

		UpdateDisplay();
	}

	private void AdjustDuration(int changeSeconds)
	{
		_durationSeconds = (int)Mathf.Max(_remainingSeconds + changeSeconds, 0);

		// Round the duration to the nearest multiple of DurationAdjustmentSeconds
		_durationSeconds = (int)(Mathf.Round(_durationSeconds / (float)DurationAdjustmentSeconds) * DurationAdjustmentSeconds);

		ResetTimer();
	}

	private void ResetTimer()
	{
		_remainingSeconds = _durationSeconds;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		float progress = _durationSeconds > 0 ? (float)(_remainingSeconds / _durationSeconds) : 0.0f;
		int displayedSeconds = (int)Mathf.Ceil(_remainingSeconds);

		_timerCircle.SetProgress(progress);
		_timerCircle.SetTime(displayedSeconds);
		_wheelsController.UpdateWheels((int)Mathf.Floor(_durationSeconds / 60), (int)(_durationSeconds % 60));
	}

}
