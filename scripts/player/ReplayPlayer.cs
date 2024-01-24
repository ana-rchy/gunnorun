using System;
using System.Diagnostics;
using Godot;
using GC = Godot.Collections;

public partial class ReplayPlayer : Node2D {
    [Export(PropertyHint.File)] string _debugPlayerScene;
    [Export] AnimatedSprite2D _sprite;
    [Export] Node2D _crosshair;
    [Export] Sprite2D _finishMarker;

    GC.Array<Vector2> _positionsList;
    GC.Array<int> _framesList;
    GC.Array<Vector2> _mousePositionsList;

    bool _unloading;

    public override void _EnterTree() {
        Paths.AddNodePath("REPLAY_PLAYER", GetPath());
    }

    public override void _Ready() {
        // change to debug if debug
        if (Global.DebugReplay && Global.ReplayOnly && FileAccess.FileExists("user://replays/debug/debug_replay.gdr")) {
            var scene = GD.Load<PackedScene>(_debugPlayerScene);
            var instance = scene.Instantiate();
            
            this.GetNodeConst("WORLD").CallDeferred("add_child", instance);
            QueueFree();
        }

        // get replay data or unload
        string replayPath = Global.ReplayName == null ? $"user://replays/{Global.CurrentWorld}_best_replay.grp"
            : $"user://imported_replays/{Global.ReplayName}";
        if (!FileAccess.FileExists(replayPath) || Multiplayer.GetPeers().Length != 0) {
            _unloading = true; // cause it still runs once when on multiplayer for some reason
            QueueFree();
            return;
        }


        ReadFromReplayFile(replayPath);
        SetPhysicsProcess(false);

        if (Global.ReplayOnly == true) {
            EmitSignal(SignalName.ReplayOnly);
            _crosshair.Show();

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
        if (_unloading) {
            return;
        }
        
        if (_replayDataIndex >= _positionsList.Count && Global.ReplayOnly == false) {
            SetPhysicsProcess(false);
            _finishMarker.Show();
            return;
        } else if (_replayDataIndex >= _positionsList.Count) {
            _replayDataIndex = 0;
        }

        Position = _positionsList[_replayDataIndex];
        _sprite.Frame = _framesList[_replayDataIndex];
        _crosshair.GlobalPosition = _mousePositionsList[_replayDataIndex];

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

        _positionsList = (GC.Array<Vector2>) dictionary["Positions"];
        _framesList = (GC.Array<int>) dictionary["Frames"];
        _mousePositionsList = (GC.Array<Vector2>) dictionary["MousePositions"];
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signal

    [Signal] public delegate void ReplayOnlyEventHandler();
    
    #endregion
}