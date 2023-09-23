using System;
using System.Collections.Generic;
using Godot;

public partial class DebugRecorder : Node {
    Player Player;

    Godot.Collections.Array<Vector2> PositionsList = new Godot.Collections.Array<Vector2>();
    Godot.Collections.Array<Vector2> VelocityList = new Godot.Collections.Array<Vector2>();
    Godot.Collections.Array<string> WeaponList = new Godot.Collections.Array<string>();

    public override void _Ready() {
        Player = GetParent<Player>();
    }

    public override void _PhysicsProcess(double delta) {
        PositionsList.Add(Player.Position);
        VelocityList.Add(Player.LinearVelocity);
        WeaponList.Add(Player.CurrentWeapon.Name);
    }

    void _OnTreeExiting() {
        Global.LastDebugData = new Godot.Collections.Dictionary<string, Variant>() {
            { "Positions", PositionsList },
            { "Velocities", VelocityList },
            { "Weapons", WeaponList }
        };
    }
}