using System;
using System.Collections.Generic;
using Godot;

public partial class Global : Node {
    public override void _Ready() {        
        DirAccess.MakeDirAbsolute("user://replays");
        DirAccess.MakeDirAbsolute("user://replays/debug");
        DirAccess.MakeDirAbsolute("user://imported_replays");
    }

    //---------------------------------------------------------------------------------//
    #region | etc

    public struct PlayerDataStruct {
        public PlayerDataStruct(string username, Color color) {
            Username = username;
            Color = color;
        }
        
        public string Username;
        public Color Color;
        public bool ReadyStatus = false;
    }

    public const float TICK_RATE = 1 / 60f;    

    public static PlayerDataStruct PlayerData = new PlayerDataStruct("", new Color(0, 0, 0, 1));
    public static Dictionary<long, PlayerDataStruct> OtherPlayerData;

    public static string CurrentWorld = "Cave";

    public static string ReplayName = null;
    public static bool ReplayOnly = false;
    public static bool DebugReplay = false;

    #endregion
}
