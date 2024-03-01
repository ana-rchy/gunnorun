using Godot;
using System;

public partial class WeaponUI : Node {
    [Export(PropertyHint.Dir)] string _weaponSpritesDir;
    [Export] TextureRect _weaponSprite;
    [Export] Label _weaponName;
    [Export] Label _ammoCount;

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnWeaponChanged(Player player) {
        //_weaponName.Text = player.CurrentWeapon.Name;
        //_ammoCount.Text = player.CurrentWeapon.Ammo.ToString();
        //_weaponSprite.Texture = (Texture2D) ResourceLoader.Load($"{_weaponSpritesDir}/{player.CurrentWeapon.Name}.png");
    }

    void _OnWeaponShot(Player player) {
        //_ammoCount.Text = player.CurrentWeapon.Ammo.ToString();
    }

    async void _OnWeaponReloading(Player player) {
        //await this.Sleep(player.CurrentWeapon.Reload);
        //_ammoCount.Text = player.CurrentWeapon.Ammo.ToString();
    }

    #endregion
}
