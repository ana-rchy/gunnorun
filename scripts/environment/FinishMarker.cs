using System;
using Godot;

public partial class FinishMarker : Node {
    Timer FinishTimer;

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        FinishTimer = GetNode<Timer>("../FinishTimer");

        FinishTimer.Timeout += GetNode<Client>(Global.SERVER_PATH)._OnFinishTimerTimeout;
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