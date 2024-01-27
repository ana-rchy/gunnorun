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
            CreateGrindingParticlesNode(i);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    // side-effects
    public void EmitMurasamaParticles() {
        Task.Run(async () => {
            _murasamaParticles.SetDeferred("emitting", true);

            await this.Sleep(0.3f);
            
            _murasamaParticles.SetDeferred("emitting", false);
        });
    }

    void CreateGrindingParticlesNode(int amount) {
        var particlesScene = Load<PackedScene>(_grindingParticlesScene);
        GrindingParticles particles = particlesScene.Instantiate<GrindingParticles>();
        IPlayer player = GetParent<IPlayer>();
        
        particles.Name = amount.ToString();
        particles.Amount = amount;
        if (player is Player) {
            ((Player) player).OnGround += particles._OnGround;
        } else if (player is PuppetPlayer) {
            ((PuppetPlayer) player).OnGround += particles._OnGround;
        }
        _grindingParticles.AddChild(particles);
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