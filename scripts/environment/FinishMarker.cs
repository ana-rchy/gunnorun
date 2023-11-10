using System;
using Godot;

public partial class FinishMarker : Node {
    [Export] Timer FinishTimer;

    public override void _Ready() {
        Paths.AddNodePath("FINISH_TIMER", FinishTimer.GetPath());

        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        FinishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void RaceFinishedEventHandler();
    
    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            EmitSignal(SignalName.RaceFinished, LevelTimer.Time);
            FinishTimer.Start();
        }
    }

    #endregion
}