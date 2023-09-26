using Godot;
using System;

public partial class PlayerUI : Node {
	public Label HP;
	public Label LevelTime;
	Label CurrentWeapon = null;

	public override void _Ready() {
		var control = GetNode("Control");

		HP = control.GetNode<Label>("HP");
		LevelTime = control.GetNode<Label>("LevelTime");

		ChangeWeapon("Shotgun");
	}

	//---------------------------------------------------------------------------------//
	#region | funcs

	public void ChangeWeapon(string weaponName) {
		if (CurrentWeapon != null) {
			CurrentWeapon.LabelSettings.FontColor = new Color("#ffffff");
		}
		CurrentWeapon = GetNode<Label>("Control/Weapons/" + weaponName);

		CurrentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
	}



	public void SetAmmoText(int? ammoCount) {
		if (ammoCount == null) {
			Ammo.Text = "infinite";
			return;
		}

		Ammo.Text = ammoCount.ToString();
	}

	public async void SetReloadText(Weapon reloadingWeapon) {
		Ammo.Text = "reloading";
		await this.Sleep(reloadingWeapon.Reload);

		var currentWeapon = GetParent<Player>().CurrentWeapon;

		if (reloadingWeapon == currentWeapon) Ammo.Text = reloadingWeapon.BaseAmmo.ToString();
	}

	#endregion
}
