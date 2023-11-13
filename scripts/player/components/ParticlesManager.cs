using System;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

public partial class ParticlesManager : Node {
    [Export(PropertyHint.File)] string _grindingParticlesScene;
    [Export] GrindingParticles _grindingParticles;
    [Export] GpuParticles2D _murasamaParticles;

    public override void _Ready() {
        for (int i = 8; i <= 64; i += 4) {
            var particlesScene = Load<PackedScene>(_grindingParticlesScene);
            GrindingParticles particles = particlesScene.Instantiate<GrindingParticles>();
            IPlayer player = GetParent<IPlayer>();
            
            particles.Name = i.ToString();
            particles.Amount = i;
            if (player is Player) {
                ((Player) player).OnGround += particles._OnGround;
            } else if (player is PuppetPlayer) {
                ((PuppetPlayer) player).OnGround += particles._OnGround;
            }
            _grindingParticles.AddChild(particles);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void EmitMurasamaParticles() {
        Task.Run(async () => {
            _murasamaParticles.SetDeferred("emitting", true);

            await this.Sleep(0.3f);
            
            _murasamaParticles.SetDeferred("emitting", false);
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