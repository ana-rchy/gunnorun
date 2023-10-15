using Godot;
using System;

public abstract class Weapon {
    public string Name;
    
    public float Knockback;
    public int Damage;
    public float Range;

    public int? Ammo, BaseAmmo;
    public float Reload;
    public float Refire;

    public float ReelbackStrength;

    public virtual void Shoot(Player player) {
        Player.LastMousePos = player.GetGlobalMousePosition();
        var mousePosToPlayerPos = Player.LastMousePos.DirectionTo(player.GlobalPosition);
        player.LinearVelocity = (player.LinearVelocity * GetMomentumMultiplier(player.LinearVelocity, mousePosToPlayerPos))
            + mousePosToPlayerPos.Normalized() * Knockback;
        // ^ get the momentum-affected velocity, and add normal weapon knockback onto it

        Ammo--;
        player.ActionTimer.Start(Refire);

        player.UI.UpdateAmmo(Name, Ammo);
        player.ShootTracer(-mousePosToPlayerPos);

        if (player.Multiplayer.GetPeers().Length != 0)
            player.CheckPlayerHit(-mousePosToPlayerPos);
    }

    float GetMomentumMultiplier(Vector2 currentVelocity, Vector2 mousePosToPlayerPos) {
        float angleDelta = currentVelocity.AngleTo(mousePosToPlayerPos);
        if (Mathf.RadToDeg(angleDelta) <= 45) // if less than 45 degrees change, keep all momentum
            return 1f;
        
        angleDelta -= MathF.Round(MathF.PI / 4, 4);

        return MathF.Round((MathF.Cos(4/3 * angleDelta) + 1) / 2, 4); // scale the momentum over a range of 135*
    }
}