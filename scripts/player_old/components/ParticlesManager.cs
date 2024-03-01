using System;
using System.Threading.Tasks;
using Godot;

public partial class ParticlesManager : Node {
    [Export] GpuParticles2D _murasamaParticles;

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
