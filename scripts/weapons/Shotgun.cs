using Godot;
using System;

public class Shotgun : Weapon {
    public Shotgun() {
        Name = "Shotgun";
        
        Knockback = 2000f;
        Refire = 0.4f;

        Damage = 25;
        Range = 500f;

        BaseAmmo = Ammo = null;
        Reload = -1f;

        ReelbackStrength = 20f;
    }
}
