using System;
using System.Collections.Generic;
using Godot;

public partial class ReplayPlayer : Node2D {
    AnimatedSprite2D Sprite;

    Godot.Collections.Array<Vector2> PositionsList;
    Godot.Collections.Array<int> FramesList;

    public override void _Ready() {
        string replayPath;
        if (Global.ReplayName == null) {
            replayPath = "user://replays/" + Global.CurrentWorld + "_best_replay.grp";
        } else {
            replayPath = "user://imported_replays/" + Global.ReplayName;
        }

        if (!FileAccess.FileExists(replayPath)) {
            QueueFree();
            return;
        }

        Sprite = GetNode<AnimatedSprite2D>("Sprite");

        using var replayFile = FileAccess.Open(replayPath, FileAccess.ModeFlags.Read);
        var dictionary = (Godot.Collections.Dictionary<string, Variant>) replayFile.GetVar();
        PositionsList = (Godot.Collections.Array<Vector2>) dictionary["Positions"];
        FramesList = (Godot.Collections.Array<int>) dictionary["Frames"];

        SetPhysicsProcess(false);
    }

    int _replayFileIndex = 0;
    public override void _PhysicsProcess(double delta) {
        if (_replayFileIndex >= PositionsList.Count) {
            SetPhysicsProcess(false);
            return;
        }

        Position = PositionsList[_replayFileIndex];
        Sprite.Frame = FramesList[_replayFileIndex];
        
        _replayFileIndex++;
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetPhysicsProcess(true);
        }
    }
}