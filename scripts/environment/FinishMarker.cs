using System;
using Godot;

public partial class FinishMarker : Node {
    [Export] Timer _finishTimer;

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }
        
        Paths.AddNodePath("FINISH_TIMER", _finishTimer.GetPath());

        RaceFinished += this.GetNodeConst<PlayerUI>("PLAYER_UI")._OnRaceFinished;
        RaceFinished += this.GetNodeConst<LevelTimer>("LEVEL_TIMER")._OnRaceFinished;
        RaceFinished += this.GetNodeConst<DebugRecorder>("DEBUG_RECORDER")._OnRaceFinished;

        _finishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void RaceFinishedEventHandler(float finishTime, string playerName);
    
    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            EmitSignal(SignalName.RaceFinished, LevelTimer.Time, "");
            _finishTimer.Start();
        }
    }

    #endregion
}