using System;
using System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

public partial class DebugRecorder : Node {
    Player Player;

    GC.Array<Vector2> PositionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> MousePositionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> VelocityList = new GC.Array<Vector2>();
    GC.Array<string> WeaponList = new GC.Array<string>();

    public override void _Ready() {
        Player = GetParent<Player>();
    }

    public override void _PhysicsProcess(double delta) {
        PositionsList.Add(Player.Position);
        MousePositionsList.Add(Player.LastMousePos);
        VelocityList.Add(Player.LinearVelocity);
        WeaponList.Add(Player.CurrentWeapon.Name);
    }

    void _OnTreeExiting() {
        Global.LastDebugData = new GC.Dictionary<string, Variant>() {
            { "Positions", PositionsList },
            { "MousePositions", MousePositionsList },
            { "Velocities", VelocityList },
            { "Weapons", WeaponList }
        };
    }
}