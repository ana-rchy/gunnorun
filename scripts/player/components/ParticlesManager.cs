using Godot;
using System;

public partial class ParticlesManager : Node {
    GpuParticles2D GrindingParticles;
    public GpuParticles2D MurasamaParticles;

    RayCast2D GrindingRaycast;

    public override void _Ready() {
        GrindingParticles = GetNode<GpuParticles2D>("GrindingParticles");
        MurasamaParticles = GetNode<GpuParticles2D>("MurasamaParticles");

        GrindingRaycast = GrindingParticles.GetNode<RayCast2D>("Raycast");
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void EmitGrinding(float xVel) {
        if (GrindingRaycast.IsColliding()) {
            var speed = MathF.Abs(xVel);

            GrindingParticles.Emitting = true;
            GrindingParticles.Amount = (int) Math.Clamp((speed / 1f), 1, 64);
        } else {
            GrindingParticles.Emitting = false;
        }
    }

    #endregion
}