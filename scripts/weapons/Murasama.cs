using System;
using System.Threading.Tasks;
using Godot;

public class Murasama : Weapon {
    public Murasama() {
        Name = "Murasama";

        Knockback = 1250f;
        Damage = 50;
        Range = 800f;

        BaseAmmo = Ammo = null;
        Reload = -1f;
        Refire = 0.5f;

        ReelbackStrength = 0f;
    }

    public override void Shoot(Player player) {
        Player.LastMousePos = player.GetGlobalMousePosition();
        var playerPosToMousePos = player.GlobalPosition.DirectionTo(Player.LastMousePos);
        player.LinearVelocity = (0.5f * player.LinearVelocity.DistanceTo(new Vector2(0, 0)) * playerPosToMousePos.Normalized())
            + playerPosToMousePos.Normalized() * Knockback;
        // ^ transfer 0.5 of previous speed into the new direction, and add on regular knock"back"

        _ = player.UpdateHP(-25);
        player.ActionTimer.Start(Refire);

        var particlesManager = player.GetNode<ParticlesManager>("Particles");
        Task.Run(async () => {
            player.SetCollisionMaskValue(4, false);
            particlesManager.MurasamaParticles.SetDeferred("emitting", true);

            await player.Sleep(0.3f);
            
            particlesManager.MurasamaParticles.SetDeferred("emitting", false);
            player.SetCollisionMaskValue(4, true);
        });

        if (player.Multiplayer.GetPeers().Length != 0)
            player.CheckPlayerHit(playerPosToMousePos);
    }
}