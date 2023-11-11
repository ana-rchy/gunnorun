using System;
using Godot;

public partial class Lap : Node {
    [Export] Timer FinishTimer;

    [Export] int MaxLaps;
    int LapCount = 1;

    public override void _Ready() {
        Paths.AddNodePath("FINISH_TIMER", FinishTimer.GetPath());
        Paths.AddNodePath("LAP", GetPath());

        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        FinishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
        EmitSignal(SignalName.LapPassed, LapCount, MaxLaps);
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void LapPassedEventHandler(int lapCount, int maxLaps);
    [Signal] public delegate void RaceFinishedEventHandler();

    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            if (LapCount < MaxLaps) {
                LapCount++;
            } else {
                EmitSignal(SignalName.RaceFinished, LevelTimer.Time);
                FinishTimer.Start();
            }
        }

        EmitSignal(SignalName.LapPassed, LapCount, MaxLaps);
    }

    #endregion
}