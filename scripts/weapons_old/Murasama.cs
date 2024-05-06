using System;
using Godot;

public class Murasama_old : Weapon_old {
    public const float INTANGIBILITY_TIME = 0.3f;

    public Murasama_old() {
        Name = "Murasama";

        Knockback = 1250f;
        Damage = 50;
        Range = 800f;

        BaseAmmo = Ammo = null;
        Reload = -1f;
        Refire = 0.5f;

        ReelbackStrength = 0f;
    }
    
    public override void Shoot(Player_old player, string tracerScene) {
        if (!player.ActionTimer.IsStopped() || player.HP <= 0) return;

        player.EmitSignal(Player_old.SignalName.WeaponShot, player);
        
        var playerPosToMousePos = player.GlobalPosition.DirectionTo(Player_old.LastMousePos);
        player.LinearVelocity = (0.5f * player.LinearVelocity.DistanceTo(new Vector2(0, 0)) * playerPosToMousePos.Normalized())
            + playerPosToMousePos.Normalized() * Knockback;
        // ^ transfer 0.5 of previous speed into the new direction, and add on regular knock"back"

        player.ChangeHP(player.GetHP() - 25, true);
        player.ActionTimer.Start(Refire);

        if (player.Multiplayer.GetPeers().Length != 0) {
            CheckPlayerHit(player, playerPosToMousePos);
        }
    }
}