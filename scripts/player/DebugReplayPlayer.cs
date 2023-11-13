using System;
using Godot;
using GC = Godot.Collections;

public partial class DebugReplayPlayer : Node2D {
    [Export] Node2D _crosshair;
    [Export] Label _debugInfo;

    GC.Array<Vector2> _positionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> _mousePositionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> _velocityList = new GC.Array<Vector2>();
    GC.Array<string> _weaponList = new GC.Array<string>();

    GC.Array<Vector2> _velocityCapList = new GC.Array<Vector2>();
    GC.Array<Single> _reelbackStrengthList = new GC.Array<Single>();
    GC.Array<Vector2> _stateVelocityList = new GC.Array<Vector2>();

    public override void _Ready() {
        this.GetNodeConst("PLAYER").QueueFree();

        using var debugFile = FileAccess.Open("user://replays/debug/debug_replay.gdr", FileAccess.ModeFlags.Read);
        var debugData = (GC.Dictionary<string, Variant>) debugFile.GetVar();

        _positionsList = (GC.Array<Vector2>) debugData["Positions"];
        _mousePositionsList = (GC.Array<Vector2>) debugData["MousePositions"];
        _velocityList = (GC.Array<Vector2>) debugData["Velocities"];
        _weaponList = (GC.Array<string>) debugData["Weapons"];

        _velocityCapList = (GC.Array<Vector2>) debugData["VelocityCaps"];
        _reelbackStrengthList = (GC.Array<Single>) debugData["ReelbackStrengths"];
        _stateVelocityList = (GC.Array<Vector2>) debugData["StateVelocities"];
    }

    int _debugDataIndex = 0;
    public override void _PhysicsProcess(double delta) {
        if (_debugDataIndex >= _positionsList.Count) {
            _debugDataIndex = 0;
        }

        var position = _positionsList[_debugDataIndex];
        var mousePosition = _mousePositionsList[_debugDataIndex];
        var velocity = _velocityList[_debugDataIndex];
        var velocityCap = _velocityCapList[_debugDataIndex];
        var stateVelocity = _stateVelocityList[_debugDataIndex];

        GlobalPosition = position;
        _crosshair.GlobalPosition = mousePosition;

        _debugInfo.Text = $"position: {position.X}, {position.Y}" +
        $"\nmouse position: {mousePosition.X}, {mousePosition.Y}" +
        $"\nvelocity: {velocity.X}, {velocity.Y}" +
        $"\nweapon: {_weaponList[_debugDataIndex]}" +
        $"\nvelocity cap: {velocityCap.X}, {velocityCap.Y}" +
        $"\nreelback strength: {_reelbackStrengthList[_debugDataIndex]}" +
        $"\nstate velocity: {stateVelocity.X}, {stateVelocity.Y}";

        _debugDataIndex++;
    }
}