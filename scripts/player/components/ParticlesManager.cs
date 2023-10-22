using System;
using System.Threading.Tasks;
using Godot;

public partial class ParticlesManager : Node {
    GpuParticles2D GrindingParticles;
    GpuParticles2D MurasamaParticles;

    public override void _Ready() {
        GrindingParticles = GetNode<GpuParticles2D>("GrindingParticles");
        MurasamaParticles = GetNode<GpuParticles2D>("MurasamaParticles");
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    public void EmitGrinding(float xVel) {
        if (GrindingRaycast.IsColliding()) {
            var speed = MathF.Abs(xVel);

            GrindingParticles.Emitting = true;
            GrindingParticles.Amount = (int) Math.Clamp((speed / 1f), 1, 64);
        } else {
            GrindingParticles.Emitting = false;
        }
    }

    void _OnWeaponShot(Player player) {
        if (player.CurrentWeapon.Name == "Murasama") {
            Task.Run(async () => {
                player.SetCollisionMaskValue(4, false);
                MurasamaParticles.SetDeferred("emitting", true);

                await player.Sleep(0.3f);
                
                MurasamaParticles.SetDeferred("emitting", false);
                player.SetCollisionMaskValue(4, true);
            });
        }
    }

    #endregion
}