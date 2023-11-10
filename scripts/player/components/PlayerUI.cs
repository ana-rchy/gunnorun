using System;
using Godot;

public partial class PlayerUI : Node {
	[Export] Label LevelTime;
	[Export] Label LapCounter;
	[Export] Label HP;
	[Export] Control RaceFinishUI;
	Label CurrentWeapon = null;

	public async override void _Ready() {
		await this.Sleep(0.01f); // HACK: no clue how else im supposed to wait until the scene loads in
		var lapManager = this.GetNodeConst<Lap>("LAP");
		if (lapManager == null) {
			LapCounter.QueueFree();
		}
	}

	//---------------------------------------------------------------------------------//
	#region | signals

	void _OnWeaponShot(Player player) {
		if (player.CurrentWeapon.Ammo == null) return;

		try {
			GetNode<Label>($"Control/Weapons/{player.CurrentWeapon.Name}/Ammo").Text
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
		var progressBar = GetNode<ProgressBar>($"Control/Weapons/{weaponName}/ProgressBar");
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

		GetNode<Label>($"Control/Weapons/{weaponName}/Ammo").Text = BaseAmmo.ToString();
	}

	void _OnWeaponChanged(string weaponName) {
		if (CurrentWeapon != null) {
			CurrentWeapon.LabelSettings.FontColor = new Color("#ffffff");
		}

		CurrentWeapon = GetNode<Label>($"Control/Weapons/{weaponName}");
		CurrentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
		
		if (weaponName == "Murasama") {
			CurrentWeapon.LabelSettings.FontColor = new Color("#e32d00");
		}
	}

	void _OnTimeChanged(float newTime) {
		LevelTime.Text = $"{Math.Round(newTime, 3)}s";
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
        LapCounter.Text = $"lap {lapCount}/{maxLaps}";
	}

	public void _OnRaceFinished(float finishTime) {
		RaceFinishUI.Show();
		RaceFinishUI.GetNode<Label>("Label").Text = $"{Math.Round(finishTime, 3)}s";
	}

	#endregion
}
