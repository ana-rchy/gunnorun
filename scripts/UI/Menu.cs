using System;
using System.Collections.Generic;
using Godot;

public partial class Menu : Node {
	public static int SelectedTab = 0;

	public override void _Ready() {
		GetNode<TabContainer>("TabContainer").CurrentTab = SelectedTab;
	}

	//---------------------------------------------------------------------------------//
    #region | signals

	void _OnTabChanged(int index) {
		SelectedTab = index;
	}

	void _OnSaveLastReplayPressed() {
		if (Global.LastReplayData != null) {
			using var replayFile = FileAccess.Open("user://imported_replays/saved_replay.grp", FileAccess.ModeFlags.Write);
			replayFile.StoreVar(Global.LastReplayData);
		}
	}
	
	void _OnHelpPressed() {
		GetNode<ColorRect>("Help/ColorRect").Show();
	}

	#endregion
}
