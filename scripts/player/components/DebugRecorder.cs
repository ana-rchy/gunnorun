using System;
using System.Collections.Generic;
using Godot;

public partial class DebugRecorder : Node {
    Player Player;
    Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> DebugData = new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>();

    public override void _Ready() {
        Player = GetParent<Player>();
    }

    public override void _PhysicsProcess(double delta) {
        DebugData.Add(new Godot.Collections.Dictionary<string, Variant>() {
            { "Position", Player.Position },
            { "Velocity", Player.LinearVelocity },
            { "Weapon", nameof(Player.CurrentWeapon) } 
        });
    }

    void _OnTreeExiting() {
        Global.LastDebugData = DebugData;
    }
}