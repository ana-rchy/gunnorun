using Godot;
using System;

public class Machinegun : Weapon {
    public Machinegun() {
        Name = "Machinegun";
        
        Knockback = 100f;
        Refire = 0.04f;

        Damage = 2;
        Range = 2500f;
        
        BaseAmmo = Ammo = 100;
        Reload = 1f;

        ReelbackStrength = 40f;
    }
}
