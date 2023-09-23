using System;
using Godot;
using GC = Godot.Collections;

public partial class DebugReplayPlayer : Node2D {
    Node2D Crosshair;
    Label DebugInfo;

    GC.Array<Vector2> PositionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> MousePositionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> VelocityList = new GC.Array<Vector2>();
    GC.Array<string> WeaponList = new GC.Array<string>();

    public override void _Ready() {
        GetNode(Global.WORLD_PATH + "Player").QueueFree();

        Crosshair = GetNode<Node2D>("Crosshair");
        DebugInfo = GetNode<Label>("DebugUI/Control/Label");

        using var debugFile = FileAccess.Open("user://replays/debug/debug_replay.gdr", FileAccess.ModeFlags.Read);
        var debugData = (GC.Dictionary<string, Variant>) debugFile.GetVar();

        PositionsList = (GC.Array<Vector2>) debugData["Positions"];
        MousePositionsList = (GC.Array<Vector2>) debugData["MousePositions"];
        VelocityList = (GC.Array<Vector2>) debugData["Velocities"];
        WeaponList = (GC.Array<string>) debugData["Weapons"];
    }

    int _debugDataIndex = 0;
    public override void _PhysicsProcess(double delta) {
        if (_debugDataIndex >= PositionsList.Count) {
            _debugDataIndex = 0;
        }

        var position = PositionsList[_debugDataIndex];
        var mousePosition = MousePositionsList[_debugDataIndex];
        var velocity = VelocityList[_debugDataIndex];

        GlobalPosition = position;
        Crosshair.GlobalPosition = mousePosition;

        DebugInfo.Text = "position: " + position.X + ", " + position.Y +
        "\nmouse position: " + mousePosition.X + ", " + mousePosition.Y +
        "\nvelocity: " + velocity.X + ", " + velocity.Y +
        "\nweapon: " + WeaponList[_debugDataIndex];

        _debugDataIndex++;
    }
}