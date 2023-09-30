using System;
using System.Collections.Generic;
using Godot;

public partial class Global : Node {
    public override void _Ready() {        
        DirAccess.MakeDirAbsolute("user://replays");
        DirAccess.MakeDirAbsolute("user://replays/debug");
        DirAccess.MakeDirAbsolute("user://imported_replays");
    }

    public override void _UnhandledInput(InputEvent e) {
		if (e.IsActionPressed("Leave")) {
			GetNode<Client>("/root/Server").LeaveServer();
		}

        if (e.IsActionPressed("Respawn")) {
            GetTree().ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
        }
	}

    //---------------------------------------------------------------------------------//
    #region | constants/structs

    public const float TICK_RATE = 1 / 60f;
    public const string WORLD_PATH = "/root/World/";
    public const string SERVER_PATH = "/root/Server/";

    public struct PlayerDataStruct {
        public PlayerDataStruct(string username, Color color) {
            Username = username;
            Color = color;
        }
        
        public string Username;
        public Color Color;
        public bool ReadyStatus = false;
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | everything else

    public static PlayerDataStruct PlayerData = new PlayerDataStruct("", new Color(0, 0, 0, 1));
    public static Dictionary<long, PlayerDataStruct> OtherPlayerData;

    public static string CurrentWorld = "Cave";

    public static double LastTime;
    public static Godot.Collections.Dictionary<string, Variant> LastReplayData = null;
    public static Godot.Collections.Dictionary<string, Variant> LastDebugData = null;

    public static string ReplayName = null;
    public static bool ReplayOnly = false;
    public static bool DebugReplay = false;

    #endregion
}
