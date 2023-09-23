using System;
using System.Collections.Generic;
using Godot;

public partial class ReplayPlayer : Node2D {
    AnimatedSprite2D Sprite;

    Godot.Collections.Array<Vector2> PositionsList;
    Godot.Collections.Array<int> FramesList;

    public override void _Ready() {
        if (Global.DebugReplay && Global.ReplayOnly && FileAccess.FileExists("user://replays/debug/debug_replay.gdr")) {
            var scene = GD.Load<PackedScene>("res://scenes/player/DebugReplayPlayer.tscn");
            var instance = scene.Instantiate();
            
            GetNode(Global.WORLD_PATH).CallDeferred("add_child", instance);
            QueueFree();
        }


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


        SetPhysicsProcess(false);

        if (Global.ReplayOnly == true) {
            GetNode(Global.WORLD_PATH + "Player").QueueFree();

            AddReplayOnlyCamera();
            SetPhysicsProcess(true);
        }

        Sprite = GetNode<AnimatedSprite2D>("Sprite");
        ReadFromReplayFile(replayPath);
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetPhysicsProcess(true);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | loop

    int _replayFileIndex = 0;
    public override void _PhysicsProcess(double delta) {
        if (_replayFileIndex >= PositionsList.Count && Global.ReplayOnly == false) {
            SetPhysicsProcess(false);
            return;
        } else if (_replayFileIndex >= PositionsList.Count) {
            _replayFileIndex = 0;
        }

        Position = PositionsList[_replayFileIndex];
        Sprite.Frame = FramesList[_replayFileIndex];

        _replayFileIndex++;
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    void AddReplayOnlyCamera() {
        var camera = new Camera2D();
        camera.Zoom = new Vector2(0.5f, 0.5f);
        camera.ProcessCallback = Camera2D.Camera2DProcessCallback.Physics;
        AddChild(camera);
    }

    void ReadFromReplayFile(string replayPath) {
        using var replayFile = FileAccess.Open(replayPath, FileAccess.ModeFlags.Read);
        var dictionary = (Godot.Collections.Dictionary<string, Variant>) replayFile.GetVar();

        PositionsList = (Godot.Collections.Array<Vector2>) dictionary["Positions"];
        FramesList = (Godot.Collections.Array<int>) dictionary["Frames"]; 
    }

    #endregion
}