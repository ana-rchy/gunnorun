using System;
using Godot;

public partial class Lap : Node {
    [Export] Timer _finishTimer;

    [Export] int _maxLaps;
    int _lapCount = 1;

    public override void _Ready() {
        Paths.AddNodePath("FINISH_TIMER", _finishTimer.GetPath());
        Paths.AddNodePath("LAP", GetPath());

        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        _finishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
        EmitSignal(SignalName.LapPassed, _lapCount, _maxLaps);
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void LapPassedEventHandler(int lapCount, int maxLaps);
    [Signal] public delegate void RaceFinishedEventHandler();

    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            if (_lapCount < _maxLaps) {
                _lapCount++;
            } else {
                EmitSignal(SignalName.RaceFinished, LevelTimer.Time);
                _finishTimer.Start();
            }
        }

        EmitSignal(SignalName.LapPassed, _lapCount, _maxLaps);
    }

    #endregion
}