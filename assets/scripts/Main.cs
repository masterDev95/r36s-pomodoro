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
	private bool _isStopped = true;

	private TimerCircle _timerCircle;
	private AudioStreamPlayer _endSound;
	private TextureRect _playPauseIcon;
	private WheelsContainer _wheelsController;
	private VBoxContainer _lrButtonsContainer;
	private Label _dpadUpLabel;
	private Label _dpadLeftLabel;
	private Label _dpadRightLabel;
	private Label _dpadDownLabel;
	private Label _aButtonLabel;
	private Label _bButtonLabel;
	private Label _xButtonLabel;
	private Label _yButtonLabel;

	public override void _Ready()
	{
		_timerCircle = GetNode<TimerCircle>("MainLayout/TimerLayout/TimerCircle");
		_endSound = GetNode<AudioStreamPlayer>("EndSound");
		_wheelsController = GetNode<WheelsContainer>("MainLayout/WheelLayout/WheelsContainer");
		_playPauseIcon = GetNode<TextureRect>("MainLayout/TimerLayout/PlayPauseContainer/PlayPauseIcon");
		_lrButtonsContainer = GetNode<VBoxContainer>("MainLayout/TimerLayout/TimerControlButtons/LRButtonsContainer");
		_dpadUpLabel = GetNode<Label>("MainLayout/WheelLayout/WheelsControlButtons/DpadContainer/Line1/DpadUpLabel");
		_dpadLeftLabel = GetNode<Label>("MainLayout/WheelLayout/WheelsControlButtons/DpadContainer/Line2/DpadLeftLabel");
		_dpadRightLabel = GetNode<Label>("MainLayout/WheelLayout/WheelsControlButtons/DpadContainer/Line2/DpadRightLabel");
		_dpadDownLabel = GetNode<Label>("MainLayout/WheelLayout/WheelsControlButtons/DpadContainer/Line3/DpadDownLabel");
		_xButtonLabel = GetNode<Label>("MainLayout/TimerLayout/TimerControlButtons/CrossButtonsContainer/Line1/XButtonLabel");
		_aButtonLabel = GetNode<Label>("MainLayout/TimerLayout/TimerControlButtons/CrossButtonsContainer/Line2/AButtonLabel");
		_yButtonLabel = GetNode<Label>("MainLayout/TimerLayout/TimerControlButtons/CrossButtonsContainer/Line2/YButtonLabel");
		_bButtonLabel = GetNode<Label>("MainLayout/TimerLayout/TimerControlButtons/CrossButtonsContainer/Line3/BButtonLabel");

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

		if (@event.IsActionPressed("timer_toggle") && _durationSeconds > 0)
		{
			_isStopped = false;
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
		// Only allow duration adjustments when the timer is stopped
		if (!_isStopped)
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
		_isRunning = isRunning;
		
		if (!_isRunning)
			_playPauseIcon.Texture = GD.Load<Texture2D>("res://assets/icons/play.svg");
		else
			_playPauseIcon.Texture = GD.Load<Texture2D>("res://assets/icons/pause.svg");
	}

	private void UpdateTimer(double delta)
	{
		if (_isRunning)
		{
			_remainingSeconds -= delta;

			if (_remainingSeconds <= 0.0)
			{
				_remainingSeconds = 0.0;
				_isStopped = true;
				SetRunningState(false);
				if (_endSound.Stream != null)
					_endSound.Play();
			}
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
		_isStopped = true;
		_remainingSeconds = _durationSeconds;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		float progress = _durationSeconds > 0 ? (float)(_remainingSeconds / _durationSeconds) : 0.0f;
		int displayedSeconds = (int)Mathf.Ceil(_remainingSeconds);

		// Show or hide the wheels layout based on whether the timer is stopped
		_wheelsController.GetParent<VBoxContainer>().Visible = _isStopped;

		// Update the D-pad labels based on the active wheel
		if (_wheelsController.ActiveWheel == Wheel.Minute)
		{
			_dpadLeftLabel.Text = "";
			_dpadRightLabel.Text = "Sec";
			_dpadUpLabel.Text = $"+{DurationAdjustmentSeconds} Min";
			_dpadDownLabel.Text = $"-{DurationAdjustmentSeconds} Min";
		}
		else
		{
			_dpadLeftLabel.Text = "Min";
			_dpadRightLabel.Text = "";
			_dpadUpLabel.Text = $"+{DurationAdjustmentSeconds} Sec";
			_dpadDownLabel.Text = $"-{DurationAdjustmentSeconds} Sec";
		}

		if (_isRunning)
		{
			_aButtonLabel.Text = "Pause";
			_bButtonLabel.Text = "";
		}
		else
		{
			_aButtonLabel.Text = "Start";
			_bButtonLabel.Text = "Reset";
		}

		if (_isStopped)
		{
			_lrButtonsContainer.Visible = true;
			_aButtonLabel.Text = "Start";
			_bButtonLabel.Text = "";
		}
		else
		{
			_lrButtonsContainer.Visible = false;
		}

		_timerCircle.SetProgress(progress);
		_timerCircle.SetTime(displayedSeconds);
		_wheelsController.UpdateWheels((int)Mathf.Floor(_durationSeconds / 60), _durationSeconds % 60);
	}

}
