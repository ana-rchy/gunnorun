using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponManager : Node2D {
    Player Player;
    Timer ActionTimer;

    public Weapon[] Weapons;
    public Weapon CurrentWeapon;

    public override void _Ready() {
        Player = GetParent<Player>();
        ActionTimer = GetNode<Timer>("ActionTimer");

        Weapons = new Weapon[] {new Shotgun(), new Machinegun(), new RPG()};
        CurrentWeapon = Weapons[0];
    }

    //---------------------------------------------------------------------------------//
    #region | main loop

    public override void _Input(InputEvent e) {
        if (e.IsActionPressed("Num1")) {
            CurrentWeapon = Weapons[0];
        } else if (e.IsActionPressed("Num2")) {
            CurrentWeapon = Weapons[1];
        } else if (e.IsActionPressed("Num3")) {
            CurrentWeapon = Weapons[2];
        }
    }

    public override void _Process(double delta) {
        var ammoNotEmpty = (CurrentWeapon.Ammo > 0 || CurrentWeapon.Ammo == null);
        
        if (Input.IsActionJustPressed("Reload") && CurrentWeapon.Ammo != CurrentWeapon.BaseAmmo) {
            CurrentWeapon.Ammo = CurrentWeapon.BaseAmmo;
            ActionTimer.Start(CurrentWeapon.Reload);

        } else if (Input.IsActionPressed("Shoot") && ActionTimer.IsStopped() && ammoNotEmpty) {
            Player.LinearVelocity = Player.GetVelocity(CurrentWeapon.Knockback);

            CurrentWeapon.Ammo--;
            ActionTimer.Start(CurrentWeapon.Refire);
        }
    }

    #endregion
}