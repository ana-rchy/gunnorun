using System;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

public partial class ParticlesManager : Node {
    GrindingParticles GrindingParticles;
    GpuParticles2D MurasamaParticles;

    public override void _Ready() {
        GrindingParticles = GetNode<GrindingParticles>("GrindingParticles");
        MurasamaParticles = GetNode<GpuParticles2D>("MurasamaParticles");

        for (int i = 8; i <= 64; i += 4) {
            var particlesScene = Load<PackedScene>("res://scenes/player/components/GrindingParticles.tscn");
            GrindingParticles particles = particlesScene.Instantiate<GrindingParticles>();
            Player player = GetParent<Player>();
            
            particles.Name = i.ToString();
            particles.Amount = i;
            player.OnGround += particles._OnGround;
            player.OffGround += particles._OffGround;
            GrindingParticles.AddChild(particles);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | signals

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