using Godot;
using System;

public partial class GrindingParticles : GpuParticles2D {
    [Export] float _maxSpeed = 2000;
    Random _rand = new();

    public override void _Ready() {
        Emitting = true;
    }
    
    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnGround(bool onGround, float xVel) {
        if (onGround) {
            var speed = MathF.Abs(xVel);

            Position = new(55 - _rand.NextSingle() * 110, Position.Y);
            AmountRatio = speed / _maxSpeed;
        } else {
            AmountRatio = 0;
        }
    }

    #endregion
}
