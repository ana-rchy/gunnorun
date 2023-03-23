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
}
