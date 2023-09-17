using System;
using System.Collections.Generic;
using Godot;
using MsgPack.Serialization;

public partial class ReplayRecorder : Node2D {
    List<Vector2> PositionsList = new List<Vector2>();

    public override void _Ready() {
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta) {
        PositionsList.Add(GlobalPosition);
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetPhysicsProcess(true);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void StopRecording() {
        SetPhysicsProcess(false);
        SaveReplay();
    }

    void SaveReplay() {
        using var replayFile = FileAccess.Open("user://replays/replay.grp", FileAccess.ModeFlags.Write);
        replayFile.StoreVar(new Godot.Collections.Array<Vector2>(PositionsList));
    }

    #endregion
}