using System;
using System.Collections.Generic;
using Godot;

public partial class Menu : Node {
	static int SelectedTab = 0;

	public override void _Ready() {
		GetNode<TabContainer>("TabContainer").CurrentTab = SelectedTab;
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
		GetNode<ColorRect>("Settings/ColorRect").Show();
	}
	
	void _OnHelpPressed() {
		GetNode<ColorRect>("Help/ColorRect").Show();
	}

	#endregion
}
