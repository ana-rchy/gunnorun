using System;
using Godot;
using GC = Godot.Collections;

public partial class ReplayRecorder : Node2D {
    [Export] AnimatedSprite2D _sprite;
    
    public static Godot.Collections.Dictionary<string, Variant> LastReplayData { get; private set; }
    GC.Array<Vector2> _positionsList = new GC.Array<Vector2>();
    GC.Array<sbyte> _framesList = new GC.Array<sbyte>();
    GC.Array<Vector2> _mousePositionsList = new GC.Array<Vector2>();

    public override void _Ready() {
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta) {
        _positionsList.Add(GlobalPosition);
        _framesList.Add((sbyte) _sprite.Frame);
        _mousePositionsList.Add(Player.LastMousePos);
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetPhysicsProcess(true);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    void SaveReplay(double finishTime) {
        using var timeFile = FileAccess.Open($"user://{Global.CurrentWorld}_time.gsd", FileAccess.ModeFlags.Write);
        timeFile.StoreDouble(finishTime);

        using var replayFile = FileAccess.Open($"user://replays/{Global.CurrentWorld}_best_replay.grp", FileAccess.ModeFlags.Write);
        replayFile.StoreVar(LastReplayData);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnRaceFinished(float finishTime) {
        SetPhysicsProcess(false);
        ProcessMode = ProcessModeEnum.Disabled;
        var timePath = $"user://{Global.CurrentWorld}_time.gsd";
        LastReplayData = new GC.Dictionary<string, Variant>() { // this is in here so it stops when it hits the finish line,
            { "World", Global.CurrentWorld },                                         // not when the scene is exited
            { "Positions", _positionsList },                                           // ...only works sometimes
            { "Frames", _framesList },
            { "MousePositions", _mousePositionsList }
        };

        if (!FileAccess.FileExists(timePath)) {
            SaveReplay(finishTime);
            return;
        }

        using var timeFile = FileAccess.Open(timePath, FileAccess.ModeFlags.Read);
        var lastBestTime = timeFile.GetDouble();
        if (finishTime < lastBestTime) {
            SaveReplay(finishTime);
        }
    }

    #endregion
}