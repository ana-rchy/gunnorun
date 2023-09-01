using Godot;
using System;

public class Shotgun : Weapon {
    public Shotgun() {
        Name = "Shotgun";
        
        Knockback = 1000f;
        Refire = 0.3f;

        Damage = 25;
        Range = 650f;

        BaseAmmo = Ammo = null;
        Reload = -1f;

        ReelbackStrength = 20f;
    }
}
