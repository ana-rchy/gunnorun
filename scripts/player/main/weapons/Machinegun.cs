using Godot;
using System;

public partial class Machinegun : Weapon {
    public Machinegun() {
        BaseAmmo = Ammo = 100;
        Knockback = 200f;

        Reload = 1f;
        Refire = 0.04f;

        Range = 1500f;
        Damage = 2;
    }
}
