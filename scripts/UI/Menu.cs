using System;
using System.Collections.Generic;
using Godot;

public partial class Menu : Node {
	[Export] TabContainer _tabContainer;
	[Export] ColorRect _settingsPanel;
	[Export] ColorRect _helpPanel;

	static int _selectedTab = 0;

	public override void _Ready() {
		_tabContainer.CurrentTab = _selectedTab;
	}

	//---------------------------------------------------------------------------------//
    #region | signals

	void _OnTabChanged(int index) {
		_selectedTab = index;
	}

	void _OnSaveLastReplayPressed() {
		if (ReplayRecorder.LastReplayData != null) {
			using var replayFile = FileAccess.Open("user://imported_replays/saved_replay.grp", FileAccess.ModeFlags.Write);
			replayFile.StoreVar(ReplayRecorder.LastReplayData);
		}
	}

	void _OnSettingsPressed() {
		_settingsPanel.Show();
	}
	
	void _OnHelpPressed() {
		_helpPanel.Show();
	}

	#endregion
}
