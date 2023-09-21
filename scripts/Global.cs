using System;
using System.Collections.Generic;
using Godot;

public static class Global {
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

        public List<Node> UnpassedCheckpoints = new List<Node>();
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | everything else

    public static PlayerDataStruct PlayerData = new PlayerDataStruct("", new Color(0, 0, 0, 1));
    public static Dictionary<long, PlayerDataStruct> OtherPlayerData;

    public static int SelectedWorldIndex = 0;
    public static string CurrentWorld = "Cave";

    public static double LastTime;
    public static Godot.Collections.Array<Vector2> LastReplayPositionsList = null;
    public static Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> LastDebugData = null;

    #endregion
}
