using Godot;
using System;

public class RPG : Weapon {
    public RPG() {
        Name = "RPG";
        
        Knockback = 11000f;
        Refire = 0.5f;

        Damage = 80;
        Range = 5000f;
        
        BaseAmmo = Ammo = 1;
        Reload = 3f;

        ReelbackStrength = 10f;
    }
}