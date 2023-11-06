using Godot;
using System;
using System.Linq;

public partial class Lap : Node {
    Timer FinishTimer;

    [Export] int MaxLaps;
    int LapCount = 0;

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        FinishTimer = GetNode<Timer>("../FinishTimer");

        FinishTimer.Timeout += GetNode<Client>(Global.SERVER_PATH)._OnFinishTimerTimeout;
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