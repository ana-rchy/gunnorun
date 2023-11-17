using System;
using Godot;

public partial class Lap : Node {
    [Export] Timer _finishTimer;

    [Export] int _maxLaps;
    int _lapCount = 1;

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        Paths.AddNodePath("FINISH_TIMER", _finishTimer.GetPath());
        Paths.AddNodePath("LAP", GetPath());

        var playerUI = this.GetNodeConst<PlayerUI>("PLAYER_UI");
        LapPassed += playerUI._OnLapPassed;
        RaceFinished += playerUI._OnRaceFinished;
        RaceFinished += this.GetNodeConst<LevelTimer>("LEVEL_TIMER")._OnRaceFinished;
        RaceFinished += this.GetNodeConst<DebugRecorder>("DEBUG_RECORDER")._OnRaceFinished;

        _finishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
        EmitSignal(SignalName.LapPassed, _lapCount, _maxLaps);
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void LapPassedEventHandler(int lapCount, int maxLaps);
    [Signal] public delegate void RaceFinishedEventHandler(float finishTime, string playerName);

    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            if (_lapCount < _maxLaps) {
                _lapCount++;
            } else {
                EmitSignal(SignalName.RaceFinished, LevelTimer.Time, "");
                _finishTimer.Start();
            }
        }

        EmitSignal(SignalName.LapPassed, _lapCount, _maxLaps);
    }

    #endregion
}