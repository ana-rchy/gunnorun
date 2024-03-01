using System;
using Godot;

public class RPG_old : Weapon_old {
    public RPG_old() {
        Name = "RPG";
        
        Knockback = 11000f;
        ReelbackStrength = 10f;
        Damage = 80;
        Range = 5000f;
        
        BaseAmmo = Ammo = 1;
        Reload = 3f;
        Refire = 0.5f;
    }
}
