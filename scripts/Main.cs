using Godot;
using System;

public partial class Main : Control
{
	public override void _Ready()
	{
		GetNode<Button>("Layout/Controls/StartPauseButton").GrabFocus();
	}

	public override void _Process(double delta)
	{
	}
}
