using Godot;
using System;

public static class Global {
    public struct PlayerDataStruct {
        public PlayerDataStruct(string username, Color color) {
            Username = username;
            Color = color;
        }
        
        public string Username;
        public Color Color;
    }
    public static PlayerDataStruct PlayerData;

    public const string WORLD_PATH = "/root/World/";
    public const float TICK_RATE = 1 / 60f;
}
