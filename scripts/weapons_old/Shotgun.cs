using System;
using Godot;

public class Shotgun_old : Weapon_old {
    public Shotgun_old() {
        Name = "Shotgun";
        
        Knockback = 1000f;
        ReelbackStrength = 20f;
        Damage = 25;
        Range = 650f;

        BaseAmmo = Ammo = null;
        Reload = -1f;
        Refire = 0.3f;
    }
}
