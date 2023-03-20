using Godot;
using System;

public partial class PlayerUI : Node {
	public Label HP;
	Label Ammo;
	public Label CurrentWeapon;

	public override void _Ready() {
		var control = GetNode("Control");

		HP = control.GetNode<Label>("HP");
		Ammo = control.GetNode<Label>("Ammo");
		CurrentWeapon = control.GetNode<Label>("CurrentWeapon");
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

	public async void SetReloadText(Weapon currentWeapon) {
		Ammo.Text = "reloading";
		await this.Sleep(currentWeapon.Reload);
		Ammo.Text = currentWeapon.BaseAmmo.ToString();
	}

	#endregion
}
