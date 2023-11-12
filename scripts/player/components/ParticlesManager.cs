using System;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

public partial class ParticlesManager : Node {
    [Export(PropertyHint.File)] string GrindingParticlesScene;
    [Export] GrindingParticles GrindingParticles;
    [Export] GpuParticles2D MurasamaParticles;

    public override void _Ready() {
        for (int i = 8; i <= 64; i += 4) {
            var particlesScene = Load<PackedScene>(GrindingParticlesScene);
            GrindingParticles particles = particlesScene.Instantiate<GrindingParticles>();
            IPlayer player = GetParent<IPlayer>();
            
            particles.Name = i.ToString();
            particles.Amount = i;
            if (player is Player) {
                ((Player) player).OnGround += particles._OnGround;
                ((Player) player).OffGround += particles._OffGround;
            } else if (player is PuppetPlayer) {
                ((PuppetPlayer) player).OnGround += particles._OnGround;
                ((PuppetPlayer) player).OffGround += particles._OffGround;
            }
            GrindingParticles.AddChild(particles);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void EmitMurasamaParticles() {
        Task.Run(async () => {
            MurasamaParticles.SetDeferred("emitting", true);

            await this.Sleep(0.3f);
            
            MurasamaParticles.SetDeferred("emitting", false);
        });
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnWeaponShot(Player player) {
        if (player.CurrentWeapon.Name == "Murasama") {
            _ = player.Intangibility(0.3f);
            EmitMurasamaParticles();
        }
    }

    #endregion
}