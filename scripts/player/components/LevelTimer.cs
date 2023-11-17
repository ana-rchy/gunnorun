using Godot;
using System;

public partial class LevelTimer : Node {
    public static double Time { get; private set; }

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            QueueFree();
        }

        Time = 0;
        SetProcess(false);
    }

    public override void _Process(double delta) {
        Time += delta;
        EmitSignal(SignalName.TimeChanged, Time);
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

    void _OnRaceFinished(float finishTime, string playerName = "") {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    #endregion
}