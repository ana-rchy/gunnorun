using System;
using System.Collections.Generic;
using Godot;

public partial class ReplayPlayer : Node2D {
    List<Vector2> PositionsList;

    public override void _Ready() {
        using var replayFile = FileAccess.Open("user://replays/replay.grp", FileAccess.ModeFlags.Read);
        PositionsList = new List<Vector2>((Godot.Collections.Array<Vector2>) replayFile.GetVar());

        SetPhysicsProcess(false);
    }

    int _replayFileIndex = 0;
    public override void _PhysicsProcess(double delta) {
        Position = PositionsList[_replayFileIndex];
        _replayFileIndex++;
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetPhysicsProcess(true);
        }
    }
}