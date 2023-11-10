using System;
using System.Diagnostics;
using Godot;
using GC = Godot.Collections;

public partial class ReplayPlayer : Node2D {
    [Export(PropertyHint.File)] string DebugPlayerScene;
    [Export] AnimatedSprite2D Sprite;
    [Export] Node2D Crosshair;
    [Export] Sprite2D FinishMarker;

    GC.Array<Vector2> PositionsList;
    GC.Array<int> FramesList;
    GC.Array<Vector2> MousePositionsList;

    public override void _Ready() {
        if (Global.DebugReplay && Global.ReplayOnly && FileAccess.FileExists("user://replays/debug/debug_replay.gdr")) {
            var scene = GD.Load<PackedScene>(DebugPlayerScene);
            var instance = scene.Instantiate();
            
            this.GetNodeConst("WORLD").CallDeferred("add_child", instance);
            QueueFree();
        }

        string replayPath = Global.ReplayName == null ? $"user://replays/{Global.CurrentWorld}_best_replay.grp"
            : $"user://imported_replays/{Global.ReplayName}";
        if (!FileAccess.FileExists(replayPath)) {
            QueueFree();
            return;
        }


        ReadFromReplayFile(replayPath);
        SetPhysicsProcess(false);

        if (Global.ReplayOnly == true) {
            EmitSignal(SignalName.ReplayOnly);
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

    int _replayDataIndex = 0;
    public override void _PhysicsProcess(double delta) {
        if (_replayDataIndex >= PositionsList.Count && Global.ReplayOnly == false) {
            SetPhysicsProcess(false);
            FinishMarker.Show();
            return;
        } else if (_replayDataIndex >= PositionsList.Count) {
            _replayDataIndex = 0;
        }

        Position = PositionsList[_replayDataIndex];
        Sprite.Frame = FramesList[_replayDataIndex];
        Crosshair.GlobalPosition = MousePositionsList[_replayDataIndex];

        _replayDataIndex++;
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

    //---------------------------------------------------------------------------------//
    #region | signal

    [Signal] public delegate void ReplayOnlyEventHandler();
    
    #endregion
}