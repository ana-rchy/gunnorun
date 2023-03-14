using Godot;
using System;

public abstract class Weapon {
    public string Name;
    
    public float Knockback;
    public float Refire;

    public int Damage;
    public float Range;

    public float? Ammo, BaseAmmo;
    public float Reload;
}