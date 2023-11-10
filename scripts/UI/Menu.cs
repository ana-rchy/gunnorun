using System;
using System.Collections.Generic;
using Godot;

public partial class Menu : Node {
	[Export] TabContainer TabContainer;
	[Export] ColorRect SettingsPanel;
	[Export] ColorRect HelpPanel;

	static int SelectedTab = 0;

	public override void _Ready() {
		TabContainer.CurrentTab = SelectedTab;
	}

	//---------------------------------------------------------------------------------//
    #region | signals

	void _OnTabChanged(int index) {
		SelectedTab = index;
	}

	void _OnSaveLastReplayPressed() {
		if (ReplayRecorder.LastReplayData != null) {
			using var replayFile = FileAccess.Open("user://imported_replays/saved_replay.grp", FileAccess.ModeFlags.Write);
			replayFile.StoreVar(ReplayRecorder.LastReplayData);
		}
	}

	void _OnSettingsPressed() {
		SettingsPanel.Show();
	}
	
	void _OnHelpPressed() {
		HelpPanel.Show();
	}

	#endregion
}
