using System;
using Godot;

public class Machinegun : Weapon {
    public Machinegun() {
        Name = "Machinegun";
        
        Knockback = 200f;
        Damage = 2;
        Range = 1500f;
        
        BaseAmmo = Ammo = 100;
        Reload = 1f;
        Refire = 0.04f;

        ReelbackStrength = 40f;
    }
}
