using System;
using Godot;

public class Machinegun_old : Weapon_old {
    public Machinegun_old() {
        Name = "Machinegun";
        
        Knockback = 200f;
        ReelbackStrength = 40f;
        Damage = 2;
        Range = 1500f;
        
        BaseAmmo = Ammo = 100;
        Reload = 1f;
        Refire = 0.04f;
    }
}
