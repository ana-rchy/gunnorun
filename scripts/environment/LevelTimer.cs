using Godot;
using System;

public partial class LevelTimer : Node {
    public double Time;

    public override void _Ready() {
        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer) {
            SetProcess(false);
        }
    }

    public override void _Process(double delta) {
        Time += delta;   
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void StopTimer() {
        SetProcess(false);
    }

    #endregion
}