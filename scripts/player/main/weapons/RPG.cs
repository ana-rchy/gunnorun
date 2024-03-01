using Godot;
using System;

public partial class RPG : Weapon {
    public RPG() {
        BaseAmmo = Ammo = 1;
        Knockback = 11000f;

        Reload = 3f;
        Refire = 0.5f;

        Range = 5000f;
        Damage = 80;
    }
}
