using Godot;
using System;

public partial class LevelTimer : Node {
    public static double Time { get; private set; }

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            QueueFree();
        }

        SetProcess(false);
    }

    public override void _Process(double delta) {
        Time += delta;
        EmitSignal(SignalName.TimeChanged, Time);
        // UI.LevelTime.Text = Math.Round(Time, 3).ToString() + "s";
    }

    public override void _PhysicsProcess(double delta) {
        if (Input.IsActionPressed("Shoot")) {
            SetProcess(true);
            SetProcessInput(false);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void TimeChangedEventHandler(float newTime);

    void _OnRaceFinished() {
        SetProcess(false);
    }

    #endregion
}