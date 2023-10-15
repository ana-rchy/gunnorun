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
		if (CurrentWeapon != null)
			CurrentWeapon.LabelSettings.FontColor = new Color("#ffffff");

		CurrentWeapon = GetNode<Label>("Control/Weapons/" + weaponName);
		CurrentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
		
		if (weaponName == "Murasama")
			CurrentWeapon.LabelSettings.FontColor = new Color("#e32d00");
	}

	public void UpdateAmmo(string weaponName, int? ammoCount) {
		if (ammoCount == null) return;

		try {
			GetNode<Label>("Control/Weapons/" + weaponName + "/Ammo").Text = ammoCount.ToString();
		} catch (Exception e) {
			if (e is ObjectDisposedException) {
				GD.Print("bad object dispose :(");
			} else {
				throw e;
			}
		}
	}

	public async void Reload(string weaponName, float reloadTime) {
		var progressBar = GetNode<ProgressBar>("Control/Weapons/" + weaponName + "/ProgressBar");
		progressBar.Show();
		var progressBarRef = new WeakReference(progressBar);

		var tweener = CreateTween();
		tweener.TweenProperty(progressBar, "value", 1, reloadTime);
		await this.Sleep(reloadTime);
		
		try {
			progressBar.Hide();
			progressBar.Value = 0;
		} catch (Exception e) {
			if (e is ObjectDisposedException) {
				GD.Print("bad object dispose :(");
			} else {
				throw e;
			}
		}
	}

	#endregion
}
