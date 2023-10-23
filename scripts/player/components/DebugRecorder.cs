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

    GC.Array<Vector2> StateVelocityList = new GC.Array<Vector2>();
    GC.Array<Vector2> VelocityCapList = new GC.Array<Vector2>();
    GC.Array<Single> ReelbackStrengthList = new GC.Array<Single>();

    public static Godot.Collections.Dictionary<string, Variant> LastDebugData { get; private set; } = null;

    public override void _Ready() {
        Player = GetParent<Player>();
    }

    public override void _PhysicsProcess(double delta) {
        PositionsList.Add(Player.Position);
        MousePositionsList.Add(Player.LastMousePos);
        VelocityList.Add(Player.LinearVelocity);
        WeaponList.Add(Player.CurrentWeapon.Name);

        StateVelocityList.Add(Player.DebugData.StateVel);
        VelocityCapList.Add(Player.DebugData.VelSoftCap);
        ReelbackStrengthList.Add(Player.DebugData.ReelbackStrength);
    }

    void _OnTreeExiting() {
        LastDebugData = new GC.Dictionary<string, Variant>() {
            { "Positions", PositionsList },
            { "MousePositions", MousePositionsList },
            { "Velocities", VelocityList },
            { "Weapons", WeaponList },

            { "StateVelocities", StateVelocityList },
            { "VelocityCaps", VelocityCapList },
            { "ReelbackStrengths", ReelbackStrengthList }
        };
    }
}