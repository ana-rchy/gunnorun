using System;
using Godot;

public partial class PlayerUI : Node {
	[Export] Label _levelTime;
	[Export] Label _lapCounter;
	[Export] Label _HP;
	[Export] Control _raceFinishUI;
	Label _currentWeapon = null;

    //---------------------------------------------------------------------------------//
    #region | signals

	public void _OnLapPassed(int lapCount, int maxLaps) {
        _lapCounter.Text = $"lap {lapCount}/{maxLaps}";
	}

	public void _OnRaceFinished(float finishTime, string playerName = null) {
		_raceFinishUI.Show();
		var raceFinishLabel = _raceFinishUI.GetNode<Label>("Label");

		raceFinishLabel.Text = $"{Math.Round(finishTime, 3)}s";
		if (playerName != null) {
			raceFinishLabel.Text = $"{playerName} has won\n" + raceFinishLabel.Text;
		}
	}

    void _OnWorldLoaded() {
		var lapManager = this.GetNodeConst<Lap>("LAP");
		if (lapManager == null) {
			_lapCounter.QueueFree();
			_lapCounter = null;
		}
	}

	void _OnWeaponShot(Player player) {
		if (_lapCounter != null) {
			_lapCounter.Show();
		}

		if (player.CurrentWeapon.Ammo == null) {
			return;
		}

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
		if (_currentWeapon != null) {
			_currentWeapon.LabelSettings.FontColor = new Color("#ffffff");
		}

		_currentWeapon = GetNode<Label>($"Control/Weapons/{weaponName}");
		_currentWeapon.LabelSettings.FontColor = new Color("#ffed4d");
		
		if (weaponName == "Murasama") {
			_currentWeapon.LabelSettings.FontColor = new Color("#e32d00");
		}
	}

	void _OnTimeChanged(float newTime) {
		_levelTime.Text = $"{Math.Round(newTime, 3)}s";
	}

	void _OnHPChanged(int newHP) {
		_HP.Text = newHP.ToString();
	}
	
	async void _OnDeath(float deathTime) {
		_HP.Text = "ur dead lol";
		await this.Sleep(deathTime);
		_HP.Text = "100";
	}

	#endregion
}
