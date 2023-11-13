using Godot;
using System;

public partial class GrindingParticles : GpuParticles2D {
    [Export] int EmissionThreshold = 80;
    Random Rand = new();

    public override void _Ready() {
        EmissionThreshold *= Amount;
    }
    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnGround(bool onGround, float xVel) {
        if (onGround) {
            var speed = MathF.Abs(xVel);

            if (speed > EmissionThreshold) {
                Position = new Vector2(Rand.Next(-55, 55+1) / 2, Position.Y);
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