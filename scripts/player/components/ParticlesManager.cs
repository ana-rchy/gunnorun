using System;
using System.Threading.Tasks;
using Godot;

public partial class ParticlesManager : Node {
    CpuParticles2D GrindingParticles;
    GpuParticles2D MurasamaParticles;

    public override void _Ready() {
        GrindingParticles = GetNode<CpuParticles2D>("GrindingParticles");
        MurasamaParticles = GetNode<GpuParticles2D>("MurasamaParticles");
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnGround(float xVel) {
        var speed = MathF.Abs(xVel);
        GD.Print(speed);

        if (speed > 250f) {
            GrindingParticles.Emitting = true;
        } else {
            GrindingParticles.Emitting = false;
        }
        GrindingParticles.Amount = (int) Math.Clamp((speed / 20f), 1, 64);
    }

    void _OffGround() {
        GrindingParticles.Emitting = false;
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