using System;
using Godot;

public class Shotgun : Weapon {
    public Shotgun() {
        Name = "Shotgun";
        
        Knockback = 1000f;
        Damage = 25;
        Range = 650f;

        BaseAmmo = Ammo = null;
        Reload = -1f;
        Refire = 0.3f;

        ReelbackStrength = 20f;
    }
}
