using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerUI : Node {
	public Label HP;
	public Label LevelTime;
	Label CurrentWeapon = null;

	public override void _Ready() {
		var control = GetNode("Control");

		HP = control.GetNode<Label>("HP");
		LevelTime = control.GetNode<Label>("LevelTime");
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

	public void UpdateAmmo(string weaponName, int? ammoCount) {
		if (ammoCount == null) return;

		GetNode<Label>("Control/Weapons/" + weaponName + "/Ammo").Text = ammoCount.ToString();
	}

	public async void Reload(string weaponName, Timer timer, float reloadTime) {
		var progressBar = GetNode<ProgressBar>("Control/Weapons/" + weaponName + "/ProgressBar");
		progressBar.Show();

		await Task.Run( () => {
			while (!timer.IsStopped()) {
				progressBar.SetDeferred("value", 1 - (timer.TimeLeft / reloadTime));
			}
		});

		progressBar.Hide();
	}

	#endregion
}
