using Godot;
using System;

public partial class Shotgun : Weapon {
    public Shotgun() {
        BaseAmmo = Ammo = null;
        Knockback = 1000f;

        Reload = -1f;
        Refire = 0.3f;

        Range = 650f;
        Damage = 25;
    }
}
