using Godot;
using System;

public abstract class Weapon {
    public string Name { get; protected set; }
    public int? Ammo { get; protected set; }
    public float Range { get; protected set; }
    public float Reload { get; protected set; }
    public bool Reloading { get; protected set; }
    
    protected float Knockback;
    protected float ReelbackStrength;
    
    protected int? BaseAmmo;
    protected float Refire;
    protected int Damage;

    public virtual void Shoot(Player player, string tracerScene) {
        if (Ammo <= 0 || !player.ActionTimer.IsStopped() || player.HP <= 0) return;

        Ammo--;
        player.EmitSignal(Player.SignalName.WeaponShot, player);

        var knockbackDirection = Player.LastMousePos.DirectionTo(player.GlobalPosition);
        player.LinearVelocity = (player.LinearVelocity * GetMomentumMultiplier(player.LinearVelocity, knockbackDirection))
            + knockbackDirection.Normalized() * Knockback;
        // ^ get the momentum-affected velocity, and add normal weapon knockback onto it
        
        //player.ApplyImpulse(knockbackDirection.Normalized() * Knockback);

        ShootTracer(player, -knockbackDirection, tracerScene);
        player.ActionTimer.Start(Refire);

        if (player.Multiplayer.GetPeers().Length != 0) {
            CheckPlayerHit(player, -knockbackDirection);
        }
    }

    public async void ReloadWeapon(Player player) {
        if (Ammo == BaseAmmo || Ammo == null || Reloading || !player.ReloadTimer.IsStopped()) return;
        Reloading = true;

        player.EmitSignal(Player.SignalName.WeaponReloading, player);
        Ammo = 0; // prevent firing remaining ammo while reloading

        player.ReloadTimer.Start(Reload); // prevent reloading in quick succession, and reloading 2+ weapons
        await player.Sleep(Reload); // prevent having ammo to fire while should be reloading

        Ammo = BaseAmmo;
        Reloading = false;
    }

    protected void CheckPlayerHit(Player player, Vector2 playerPosToMousePos) {
        player.WeaponRaycast.TargetPosition = playerPosToMousePos.Normalized() * Range;
        player.WeaponRaycast.ForceRaycastUpdate();

        if (player.WeaponRaycast.IsColliding()) {
            var hitPlayer = (PuppetPlayer) player.WeaponRaycast.GetCollider();
            player.EmitSignal(Player.SignalName.OtherPlayerHit, long.Parse(hitPlayer.Name), hitPlayer.HP - Damage, Name);
        }
    }

    // pure!!!
    float GetMomentumMultiplier(Vector2 currentVelocity, Vector2 mousePosToPlayerPos) {
        float angleDelta = currentVelocity.AngleTo(mousePosToPlayerPos);
        if (Mathf.RadToDeg(angleDelta) <= 45) { // if less than 45 degrees change, keep all momentum
            return 1f;
        }
        
        angleDelta -= MathF.Round(MathF.PI / 4, 4);

        return MathF.Round((MathF.Cos(4/3 * angleDelta) + 1) / 2, 4); // scale the momentum over a range of 135*
    }

    void ShootTracer(Player player, Vector2 playerPosToMousePos, string tracerScene) {
        var tracer = GD.Load<PackedScene>(tracerScene).Instantiate<Tracer>();

        tracer.GlobalPosition = player.GlobalPosition;
        tracer.Rotation = new Vector2(0, 0).AngleToPoint(playerPosToMousePos);
        tracer.Range = Range;

        player.AddSibling(tracer);
    }
}
