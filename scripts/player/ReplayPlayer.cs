using System;
using System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

public partial class ReplayPlayer : Node2D {
    AnimatedSprite2D Sprite;
    Node2D Crosshair;

    GC.Array<Vector2> PositionsList;
    GC.Array<int> FramesList;
    GC.Array<Vector2> MousePositionsList;

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


        Sprite = GetNode<AnimatedSprite2D>("Sprite");
        Crosshair = GetNode<Node2D>("Crosshair");

        ReadFromReplayFile(replayPath);
        SetPhysicsProcess(false);

        if (Global.ReplayOnly == true) {
            GetNode(Global.WORLD_PATH + "Player").QueueFree();
            Crosshair.Show();

            AddReplayOnlyCamera();
            SetPhysicsProcess(true);
        }
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
        Crosshair.GlobalPosition = MousePositionsList[_replayFileIndex];

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
        var dictionary = (GC.Dictionary<string, Variant>) replayFile.GetVar();

        PositionsList = (GC.Array<Vector2>) dictionary["Positions"];
        FramesList = (GC.Array<int>) dictionary["Frames"];
        MousePositionsList = (GC.Array<Vector2>) dictionary["MousePositions"];
    }

    #endregion
}