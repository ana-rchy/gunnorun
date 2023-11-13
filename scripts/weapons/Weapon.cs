using Godot;
using System;

public abstract class Weapon {
    public string Name { get; protected set; }
    public int? Ammo { get; protected set; }
    public float Range { get; protected set; }
    
    protected float Knockback;
    protected float ReelbackStrength;
    
    protected int? BaseAmmo;
    protected float Reload;
    protected float Refire;
    protected int Damage;

    public virtual void Shoot(Player player) {
        if (Ammo <= 0) {
            return;
        }
        Ammo--;
        player.EmitSignal(Player.SignalName.WeaponShot, player);

        var mousePosToPlayerPos = Player.LastMousePos.DirectionTo(player.GlobalPosition);
        player.LinearVelocity = (player.LinearVelocity * GetMomentumMultiplier(player.LinearVelocity, mousePosToPlayerPos))
            + mousePosToPlayerPos.Normalized() * Knockback;
        // ^ get the momentum-affected velocity, and add normal weapon knockback onto it

        ShootTracer(player, -mousePosToPlayerPos);
        player.ActionTimer.Start(Refire);

        if (player.Multiplayer.GetPeers().Length != 0) {
            CheckPlayerHit(player, -mousePosToPlayerPos);
        }
    }

    public async void ReloadWeapon(Player player) {
        if (Ammo == 100 || Ammo == null) {
            return;
        }

        player.EmitSignal(Player.SignalName.WeaponReloading, Name, Reload, (int) BaseAmmo);
        Ammo = 0; // prevent firing remaining ammo while reloading

        player.ReloadTimer.Start(); // prevent reloading in quick succession, and reloading 2+ weapons
        await player.Sleep(Reload); // prevent having ammo to fire while should be reloading

        Ammo = BaseAmmo;
    }

    protected void CheckPlayerHit(Player player, Vector2 playerPosToMousePos) {
        player.WeaponRaycast.TargetPosition = playerPosToMousePos.Normalized() * Range;
        player.WeaponRaycast.ForceRaycastUpdate();

        if (player.WeaponRaycast.IsColliding()) {
            var hitPlayer = (PuppetPlayer) player.WeaponRaycast.GetCollider();
            player.EmitSignal(Player.SignalName.OtherPlayerHit, long.Parse(hitPlayer.Name), hitPlayer.HP - Damage, Name);
        }
    }

    float GetMomentumMultiplier(Vector2 currentVelocity, Vector2 mousePosToPlayerPos) {
        float angleDelta = currentVelocity.AngleTo(mousePosToPlayerPos);
        if (Mathf.RadToDeg(angleDelta) <= 45) { // if less than 45 degrees change, keep all momentum
            return 1f;
        }
        
        angleDelta -= MathF.Round(MathF.PI / 4, 4);

        return MathF.Round((MathF.Cos(4/3 * angleDelta) + 1) / 2, 4); // scale the momentum over a range of 135*
    }

    void ShootTracer(Player player, Vector2 playerPosToMousePos) {
        var tracerScene = GD.Load<PackedScene>(player.TracerScene);
        var tracer = tracerScene.Instantiate<Tracer>();

        tracer.GlobalPosition = player.GlobalPosition;
        tracer.Rotation = new Vector2(0, 0).AngleToPoint(playerPosToMousePos);
        tracer.Range = Range;

        player.AddSibling(tracer);
    }
}