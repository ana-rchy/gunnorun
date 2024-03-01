using Godot;
using System;

public partial class Murasama : Weapon {
    public const float INTANGIBILITY_TIME = 0.3f;

    public Murasama() {
        BaseAmmo = Ammo = null;
        Knockback = 1250f;

        Reload = -1f;
        Refire = 0.5f;

        Range = 800f;
        Damage = 50;
    }
}
