# R36S Pomodoro

A lightweight Pomodoro-style timer built with Godot 4 and C# for the R36S handheld console.

The app focuses on a single-screen countdown interface with a circular progress ring, adjustable minute/second duration, and simple controls designed for quick, distraction-free timing sessions.

## Features

- Adjustable timer length using minute and second wheels
- Circular countdown visualization
- Start/pause toggle and reset actions
- Quick duration changes while the timer is stopped
- R36S-friendly layout with large, readable controls
- Completion sound when the timer reaches zero

## Controls

The project is designed around the R36S gamepad layout, but the same behavior can be triggered from the on-screen logic and input actions.

- A button: start or pause the timer
- B button: reset the current timer to its selected duration
- Left / Right: switch between the minute and second wheel
- Up / Down: increase or decrease the selected wheel value
- L1 / R1 or turbo actions: fine-tune the duration in quick steps

## Requirements

- Godot 4.7.2
- .NET SDK 8.0 or newer
- An R36S-compatible handheld or a local desktop environment to run the Godot project

## Installation

### On the R36S

1. Download the latest release from [GitHub Releases](https://github.com/masterDev95/r36s-pomodoro/releases).
2. Extract the release archive into the ports folder on your device:

```bash
/roms/ports
```

3. Launch the application from the R36S ports menu or launcher.

### From source

1. Clone the repository:

```bash
git clone https://github.com/masterDev95/r36s-pomodoro.git
cd r36s-pomodoro
```

2. Open the `project.godot` file in Godot 4.7.2.
3. Ensure the C# project is recognized by Godot.
4. Press Run to launch the project locally.

## Exporting for R36S

The `R36S` preset exports the ARM64 game to `build/pomodoro/`. To create the complete PortMaster layout, including the `pomodoro.sh` launcher, run:

```bash
./export-r36s.sh
```

The resulting layout is:

```text
build/
├── pomodoro.sh
└── pomodoro/
    ├── pomodoro.arm64
    ├── pomodoro.pck
    └── data_Visual Timer_linuxbsd_arm64/
```

The Godot .NET executable must be available as `godot` on `PATH`. If it has another name or location, set `GODOT_BIN` when exporting, for example `GODOT_BIN=/path/to/godot ./export-r36s.sh`. The separate `Linux x86_64` preset remains available for desktop builds.

Generated files under `build/` are excluded from the project resources so previous exports are not embedded in the new PCK.

## Project structure

- `main.tscn` — main UI scene
- `assets/scripts/Main.cs` — timer logic and input handling
- `assets/scripts/WheelsContainer.cs` — custom minute/second wheel controls
- `assets/scripts/TimerCircle.cs` — circular progress rendering
- `export_presets.cfg` — Godot export configuration for R36S builds

## License

This project is free to use and free of rights.
