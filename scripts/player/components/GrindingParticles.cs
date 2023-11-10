using Godot;
using System;

public partial class GrindingParticles : GpuParticles2D {
    Random Rand = new();
    int EmissionThreshold;

    public override void _Ready() {
        EmissionThreshold = 80 * Amount;
    }
    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnGround(float xVel) {
        var speed = MathF.Abs(xVel);

        if (speed > EmissionThreshold) {
            Position = new Vector2(Rand.Next(-55, 55+1) / 2, Position.Y);
            Emitting = true;
        } else {
            Emitting = false;
        }
        // GrindingParticles.Amount = (int) Math.Clamp((speed / 20f), 1, 64);
    }
    
    public void _OffGround() {
        Emitting = false;
    }

    #endregion
}