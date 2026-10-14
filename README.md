# R36S Pomodoro

A lightweight Pomodoro timer built with Godot 4 and C# specifically designed to run on the R36S handheld console.

This project displays a circular visual timer with progress tracking, start/pause controls, a reset button, and quick duration adjustments. It is designed to be simple, focused, and easy to use for productivity sessions on the R36S.

## Features

- Default Pomodoro timer set to 25 minutes
- Quick time adjustments: +5 min / -5 min
- Immediate timer reset
- Circular visual progress indicator
- Optimized for use on the R36S handheld console
## Requirements

- Godot 4.7.2
- .NET SDK 8.0 or newer
## Installation

### On the R36S

1. Download the latest release from [here](https://github.com/masterDev95/r36s-pomodoro/releases)

2. Extract the `pomodoro.zip` archive into the following folder on your device:

```bash
/roms/ports
```

3. Launch the game from the R36S ports menu or launcher.

### From source

1. Clone the repository:

```bash
git clone <repository-url>
cd r36s-pomodoro
```

2. Open the `project.godot` file with Godot.

3. Make sure the C# project is recognized correctly by Godot.

4. Select the main scene and run the project.

## Usage

- Click `START` to begin the timer.
- Click `PAUSE` to suspend the countdown.
- Click `RESET` to restore the timer to its current duration.
- Use `+5 MIN` and `-5 MIN` to adjust the session time.
- Progress is shown as a circular arc representing the remaining time.

## Technical Notes

- The timer logic is implemented in `scripts/Main.cs`.
- The circular progress rendering is handled in `scripts/TimerCircle.cs`.
- The main scene is defined in `main.tscn`.
- The project uses the Godot .NET SDK (`Godot.NET.Sdk/4.7.2`).

## License

This project is free to use and free of rights.
