using System;
using Godot;
using MsgPack.Serialization;

public partial class ReplayRecorder : Node2D {
    AnimatedSprite2D Sprite;
    
    Godot.Collections.Array<Vector2> PositionsList = new Godot.Collections.Array<Vector2>();
    Godot.Collections.Array<sbyte> FramesList = new Godot.Collections.Array<sbyte>();

    public override void _Ready() {
        SetPhysicsProcess(false);

        Sprite = GetNode<AnimatedSprite2D>("../Sprite");
    }

    public override void _PhysicsProcess(double delta) {
        PositionsList.Add(GlobalPosition);
        FramesList.Add((sbyte) Sprite.Frame);
    }

    public override void _Input(InputEvent e) {
        if (Input.IsActionPressed("Shoot")) {
            SetPhysicsProcess(true);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void StopRecording(double finalTime) {
        SetPhysicsProcess(false);
        var timePath = "user://" + Global.CurrentWorld + "_time.gsd";

        if (FileAccess.FileExists(timePath)) {
            using var timeFile = FileAccess.Open(timePath, FileAccess.ModeFlags.Read);
            var lastBestTime = timeFile.GetDouble();
            
            if (finalTime < lastBestTime) {
                SaveReplay(finalTime);
            }
        } else {
            SaveReplay(finalTime);
        }
    }

    void SaveReplay(double finalTime) {
        using var timeFile = FileAccess.Open("user://" + Global.CurrentWorld + "_time.gsd", FileAccess.ModeFlags.Write);
        timeFile.StoreDouble(finalTime);

        using var replayFile = FileAccess.Open("user://replays/" + Global.CurrentWorld + "_best_replay.grp", FileAccess.ModeFlags.Write);
        Global.LastReplayData = new Godot.Collections.Dictionary<string, Variant>() { // this is in here so it stops when it hits the finish line,
            { "World", Global.CurrentWorld },                                         // not when the scene is exited
            { "Positions", PositionsList },                                           // ...only works sometimes
            { "Frames", FramesList }
        };
        replayFile.StoreVar(Global.LastReplayData);
    }

    #endregion
}