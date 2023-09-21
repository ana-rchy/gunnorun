using System;
using System.Collections.Generic;
using Godot;

public partial class Menu : Node {
	void _OnSaveLastReplayPressed() {
		if (Global.LastReplayData != null) {
			using var replayFile = FileAccess.Open("user://replays/saved_replay.grp", FileAccess.ModeFlags.Write);
			replayFile.StoreVar(Global.LastReplayData);
		}
	}
	
	void _OnSaveDebugData() {
		if (Global.LastDebugData != null) {
			using var debugFile = FileAccess.Open("user://replays/debug_replay.gdr", FileAccess.ModeFlags.Write);
			debugFile.StoreVar(Global.LastDebugData);
		}
	}
}
