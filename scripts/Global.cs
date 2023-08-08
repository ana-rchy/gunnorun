using Godot;
using System;
using System.Collections.Generic;

public static class Global {
    public struct PlayerDataStruct {
        public PlayerDataStruct(string username, Color color) {
            Username = username;
            Color = color;
        }
        
        public string Username;
        public Color Color;
        public bool ReadyStatus = false;
    }
    public static PlayerDataStruct PlayerData;
    public static Dictionary<long, PlayerDataStruct> OtherPlayerData;

    public const float TICK_RATE = 1 / 60f;
    public const string WORLD_PATH = "/root/Main/World/";
    public const string SERVER_PATH = "/root/Server/";
}
