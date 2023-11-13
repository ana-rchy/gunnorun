using Godot;
using System;

public partial class GrindingParticles : GpuParticles2D {
    [Export] int _emissionThreshold = 80;
    Random _rand = new();

    public override void _Ready() {
        _emissionThreshold *= Amount;
    }
    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnGround(bool onGround, float xVel) {
        if (onGround) {
            var speed = MathF.Abs(xVel);

            if (speed > _emissionThreshold) {
                Position = new Vector2(_rand.Next(-55, 55+1) / 2, Position.Y);
                Emitting = true;
            } else {
                Emitting = false;
            }
        } else {
            Emitting = false;
        }
    }

    #endregion
}