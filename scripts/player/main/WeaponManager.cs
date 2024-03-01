using System;
using System.Collections.Generic;
using Godot;

public partial class WeaponManager : Node {
    [Export(PropertyHint.File)] string _tracerScene;
    [Export] Player _player;

    public Weapon CurrentWeapon { get; private set; }
    static string CurrentWeaponName = "Shotgun"; // needed to preserve weapon choice, but w/o weapon data
    List<Weapon> _weapons;

    public override void _Ready() {
        _weapons = GetChildren();
    }

    //---------------------------------------------------------------------------------//
    #region | loops

    public override void _Input(InputEvent e) {
        for (int i = 1; i <= 4; i++) {
            if (e.IsActionPressed($"Num{i}")) {
                CurrentWeapon = _weapons[i-1];
                CurrentWeaponIndex = i-1;

                EmitSignal(SignalName.WeaponChanged, this);
            }
        }

        if (e.IsActionPressed("NextWeapon")) {
            CurrentWeaponIndex = (CurrentWeaponIndex + 1) % _weapons.Length;
            CurrentWeapon = _weapons[CurrentWeaponIndex];
            EmitSignal(SignalName.WeaponChanged, this);
        } else if (e.IsActionPressed("PreviousWeapon")) {
            CurrentWeaponIndex = (CurrentWeaponIndex - 1) % _weapons.Length;
            CurrentWeapon = _weapons[CurrentWeaponIndex];
            EmitSignal(SignalName.WeaponChanged, this);
        }

        if (Input.IsActionJustPressed("Reload")) {
            CurrentWeapon.ReloadWeapon(_player);
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (Input.IsActionPressed("Shoot")) {
            CurrentWeapon.Shoot(_player, _tracerScene);
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void WeaponChangedEventHandler(Player_old player);
    [Signal] public delegate void WeaponShotEventHandler(Player_old player);
    [Signal] public delegate void WeaponReloadingEventHandler(Player_old player);

    #endregion
}
