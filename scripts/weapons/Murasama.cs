using System;
using Godot;

public class Murasama : Weapon {
    public Murasama() {
        Name = "Murasama";

        Knockback = 1250f;
        Damage = 50;
        Range = 500f;

        BaseAmmo = Ammo = null;
        Reload = -1f;
        Refire = 1f;

        ReelbackStrength = 0f;
    }

    public override void Shoot(Player player) {
        Player.LastMousePos = player.GetGlobalMousePosition();
        var playerPosToMousePos = player.GlobalPosition.DirectionTo(Player.LastMousePos);
        player.LinearVelocity = (0.5f * player.LinearVelocity.DistanceTo(new Vector2(0, 0)) * playerPosToMousePos.Normalized())
            + playerPosToMousePos.Normalized() * Knockback;
        // ^ get the momentum-affected velocity, and add normal weapon knockback onto it

        player.UpdateHP(-25);
        player.ActionTimer.Start(Refire);

        if (player.Multiplayer.GetPeers().Length != 0) {
            player.CheckPlayerHit(-playerPosToMousePos);
        }
    }
}