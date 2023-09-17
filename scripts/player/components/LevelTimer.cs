using Godot;
using System;

public partial class LevelTimer : Node {
    PlayerUI UI;

    public double Time;

    public override void _Ready() {
        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer) {
            QueueFree();
        }

        SetProcess(false);
        UI = GetNode<PlayerUI>("../PlayerUI");
    }

    public override void _Process(double delta) {
        Time += delta;
        UI.LevelTime.Text = Math.Round(Time, 3).ToString() + "s";
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetProcess(true);
            SetProcessInput(false);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public double StopTimer() {
        SetProcess(false);
        Global.LastTime = Math.Round(Time, 3);
        return Global.LastTime;
    }

    #endregion
}