using Godot;
using System;

public partial class MovementManager : Node {
    [Export] Player _player;

    const float VEL_SOFT_CAP = 4000f;
    const float DRAG_MULTIPLIER = 1f;

    public override void _PhysicsProcess(double delta) {
        // drag force
        // desmos: c(ap) = [any], m(ultiplier) = [any], f(x) = x / (x + (c/m))
        var linearVel = _player.LinearVelocity;
        var excessSpeed = Math.Clamp(linearVel.Length() - VEL_SOFT_CAP, 0, double.PositiveInfinity);
        _player.ApplyForce(-linearVel.Normalized() * (float) (excessSpeed / (excessSpeed + VEL_SOFT_CAP/DRAG_MULTIPLIER)));        
    }
}
