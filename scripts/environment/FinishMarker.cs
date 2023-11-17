using System;
using Godot;

public partial class FinishMarker : Node {
    [Export] Timer _finishTimer;

    public override void _Ready() {
        Paths.AddNodePath("FINISH_TIMER", _finishTimer.GetPath());

        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        _finishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void RaceFinishedEventHandler();
    
    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            EmitSignal(SignalName.RaceFinished, LevelTimer.Time, "");
            _finishTimer.Start();
        }
    }

    #endregion
}