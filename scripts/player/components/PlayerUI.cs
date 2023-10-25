using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerUI : Node {
	Label LevelTime;
	Label LapCounter;
	Label HP;
	Label CurrentWeapon = null;

	public override void _Ready() {
		LevelTime = GetNode<Label>("Control/LevelTime");
		LapCounter = GetNode<Label>("Control/LapCounter");
		HP = GetNode<Label>("Control/HP");
		
		var lapManager = GetNodeOrNull<Lap>(Global.WORLD_PATH + "Markers/Lap");
		if (lapManager == null)
			LapCounter.QueueFree();
		// } else {
		// 	LapCounter.Text = "lap 1/" + lapManager.MaxLaps.ToString();
		// }
	}

	//---------------------------------------------------------------------------------//
	#region | signals

	void _OnWeaponShot(Player player) {
		if (player.CurrentWeapon.Ammo == null) return;

		try {
			GetNode<Label>("Control/Weapons/" + player.CurrentWeapon.Name + "/Ammo").Text
				= player.CurrentWeapon.Ammo.ToString();
		} catch (Exception e) {
			if (e is ObjectDisposedException) {
				GD.Print("bad object dispose :(");
			} else {
				throw;
			}
		}
	}

	async void _OnWeaponReloading(string weaponName, float reloadTime, int BaseAmmo) {
		var progressBar = GetNode<ProgressBar>("Control/Weapons/" + weaponName + "/ProgressBar");
		progressBar.Show();

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
				throw;
			}
		}

		GetNode<Label>("Control/Weapons/" + weaponName + "/Ammo").Text = BaseAmmo.ToString();
	}

	void _OnWeaponChanged(string weaponName) {
		if (CurrentWeapon != null)
			CurrentWeapon.LabelSettings.FontColor = new Color("#ffffff");

		CurrentWeapon = GetNode<Label>("Control/Weapons/" + weaponName);
		CurrentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
		
		if (weaponName == "Murasama")
			CurrentWeapon.LabelSettings.FontColor = new Color("#e32d00");
	}

	void _OnTimeChanged(float newTime) {
		LevelTime.Text = Math.Round(newTime, 3).ToString() + "s";
	}

	void _OnHPChanged(int newHP) {
		HP.Text = newHP.ToString();
	}
	
	async void _OnDeath(float deathTime) {
		HP.Text = "ur dead lol";
		await this.Sleep(deathTime);
		HP.Text = "100";
	}

	public void _OnLapPassed(int lapCount, int maxLaps) {
        LapCounter.Text = "lap " + lapCount.ToString() + "/" + maxLaps.ToString();
	}

	public void _OnRaceFinished(float finishTime) {
		var raceFinishUI = GetNode<Control>("Control/RaceFinish");
		raceFinishUI.Show();
		raceFinishUI.GetNode<Label>("Label").Text = Math.Round(finishTime, 3).ToString() + "s";
	}

	// public void ChangeWeapon(string weaponName) {
	// 	if (CurrentWeapon != null)
	// 		CurrentWeapon.LabelSettings.FontColor = new Color("#ffffff");

	// 	CurrentWeapon = GetNode<Label>("Control/Weapons/" + weaponName);
	// 	CurrentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
		
	// 	if (weaponName == "Murasama")
	// 		CurrentWeapon.LabelSettings.FontColor = new Color("#e32d00");
	// }

	// public void UpdateAmmo(string weaponName, int? ammoCount) {
	// 	if (ammoCount == null) return;

	// 	try {
	// 		GetNode<Label>("Control/Weapons/" + weaponName + "/Ammo").Text = ammoCount.ToString();
	// 	} catch (Exception e) {
	// 		if (e is ObjectDisposedException) {
	// 			GD.Print("bad object dispose :(");
	// 		} else {
	// 			throw e;
	// 		}
	// 	}
	// }	

	// public async void Reload(string weaponName, float reloadTime) {
	// 	var progressBar = GetNode<ProgressBar>("Control/Weapons/" + weaponName + "/ProgressBar");
	// 	progressBar.Show();
	// 	var progressBarRef = new WeakRefer   public static double LastTime;ence(progressBar);

	// 	var tweener = CreateTween();
	// 	tweener.TweenProperty(progressBar, "value", 1, reloadTime);
	// 	await this.Sleep(reloadTime);
		
	// 	try {
	// 		progressBar.Hide();
	// 		progressBar.Value = 0;
	// 	} catch (Exception e) {
	// 		if (e is ObjectDisposedException) {
	// 			GD.Print("bad object dispose :(");
	// 		} else {
	// 			throw e;
	// 		}
	// 	}
	// }

	#endregion
}
