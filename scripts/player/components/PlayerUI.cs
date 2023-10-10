using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerUI : Node {
	public Label HP;
	public Label LapCounter;
	public Label LevelTime;

	Label CurrentWeapon = null;

	public override void _Ready() {
		LevelTime = GetNode<Label>("Control/LevelTime");
		LapCounter = GetNode<Label>("Control/LapCounter");
		HP = GetNode<Label>("Control/HP");
		
		var lapManager = GetNodeOrNull<Lap>(Global.WORLD_PATH + "Markers/Lap");
		if (lapManager == null) {
			LapCounter.QueueFree();
		} else {
			LapCounter.Text = "lap 1/" + lapManager.MaxLaps.ToString();
		}
	}

	//---------------------------------------------------------------------------------//
	#region | funcs

	public void ChangeWeapon(string weaponName) {
		if (CurrentWeapon != null) {
			CurrentWeapon.LabelSettings.FontColor = new Color("#ffffff");
		}
		CurrentWeapon = GetNode<Label>("Control/Weapons/" + weaponName);

		CurrentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
		if (weaponName == "Murasama")
			CurrentWeapon.LabelSettings.FontColor = new Color("#e32d003");
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
