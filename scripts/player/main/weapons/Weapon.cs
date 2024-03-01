using Godot;
using System;

public partial class Weapon : Node {
    public int? Ammo { get; protected set; }
    protected int? BaseAmmo;
    protected float Knockback;

    public float Reload { get; protected set; }
    public bool Reloading { get; protected set; }
    protected float Refire;

    public float Range { get; protected set; }
    protected int Damage;
}
