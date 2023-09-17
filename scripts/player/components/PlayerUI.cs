using Godot;
using System;

public partial class PlayerUI : Node {
	public Label HP;
	public Label SelectedWeapon;
	public Label ReloadingWarning;
	public Label LevelTime;
	Label Ammo;

	public override void _Ready() {
		var control = GetNode("Control");

		HP = control.GetNode<Label>("HP");
		SelectedWeapon = control.GetNode<Label>("CurrentWeapon");
		ReloadingWarning = control.GetNode<Label>("ReloadingWarning");
		Ammo = control.GetNode<Label>("Ammo");
		LevelTime = control.GetNode<Label>("LevelTime");
	}

	//---------------------------------------------------------------------------------//
	#region | funcs

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
