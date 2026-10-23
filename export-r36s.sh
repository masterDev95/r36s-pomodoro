#!/usr/bin/env bash
set -euo pipefail

project_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
godot_bin="${GODOT_BIN:-godot}"

if ! command -v "$godot_bin" >/dev/null 2>&1; then
  printf 'Godot executable not found: %s\nSet GODOT_BIN to the Godot .NET executable.\n' "$godot_bin" >&2
  exit 1
fi

mkdir -p "$project_dir/build/pomodoro"
"$godot_bin" --headless --path "$project_dir" --export-release "R36S"
install -D -m 755 "$project_dir/portmaster/pomodoro.sh" "$project_dir/build/pomodoro.sh"

printf 'R36S port exported to %s/build\n' "$project_dir"
