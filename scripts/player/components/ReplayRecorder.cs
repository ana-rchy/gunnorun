using System;
using Godot;
using GC = Godot.Collections;
using MsgPack.Serialization;

public partial class ReplayRecorder : Node2D {
    AnimatedSprite2D Sprite;
    
    GC.Array<Vector2> PositionsList = new GC.Array<Vector2>();
    GC.Array<sbyte> FramesList = new GC.Array<sbyte>();
    GC.Array<Vector2> MousePositionsList = new GC.Array<Vector2>();
    public static Godot.Collections.Dictionary<string, Variant> LastReplayData { get; private set; }

    public override void _Ready() {
        SetPhysicsProcess(false);

        Sprite = GetNode<AnimatedSprite2D>("../Sprite");
    }

    public override void _PhysicsProcess(double delta) {
        PositionsList.Add(GlobalPosition);
        FramesList.Add((sbyte) Sprite.Frame);
        MousePositionsList.Add(Player.LastMousePos);
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot"))
            SetPhysicsProcess(true);
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    void SaveReplay(double finishTime) {
        using var timeFile = FileAccess.Open("user://" + Global.CurrentWorld + "_time.gsd", FileAccess.ModeFlags.Write);
        timeFile.StoreDouble(finishTime);

        using var replayFile = FileAccess.Open("user://replays/" + Global.CurrentWorld + "_best_replay.grp", FileAccess.ModeFlags.Write);
        replayFile.StoreVar(LastReplayData);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnRaceFinished(float finishTime) {
        SetPhysicsProcess(false);
        var timePath = "user://" + Global.CurrentWorld + "_time.gsd";
        LastReplayData = new GC.Dictionary<string, Variant>() { // this is in here so it stops when it hits the finish line,
            { "World", Global.CurrentWorld },                                         // not when the scene is exited
            { "Positions", PositionsList },                                           // ...only works sometimes
            { "Frames", FramesList },
            { "MousePositions", MousePositionsList }
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