using Godot;
using System;

public partial class WeaponManager : Node {
    [Export(PropertyHint.File)] public string TracerScene { get; private set; }
    [Export] Player _player;

    public Weapon CurrentWeapon { get; private set; }
    static int CurrentWeaponIndex = 0; // needed to preserve weapon choice, but w/o weapon data
    Weapon[] _weapons = new Weapon[] { new Shotgun(), new Machinegun(), new RPG(), new Murasama() };

    public override void _Ready() {
        CurrentWeapon = _weapons[CurrentWeaponIndex];
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
            CurrentWeapon.Shoot(_player);
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void WeaponChangedEventHandler(Player player);
    [Signal] public delegate void WeaponShotEventHandler(Player player);
    [Signal] public delegate void WeaponReloadingEventHandler(Player player);

    #endregion
}
