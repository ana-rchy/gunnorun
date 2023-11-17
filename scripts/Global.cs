using System;
using System.Collections.Generic;
using Godot;

public partial class Global : Node {
    public override void _Ready() {        
        DirAccess.MakeDirAbsolute("user://replays");
        DirAccess.MakeDirAbsolute("user://replays/debug");
        DirAccess.MakeDirAbsolute("user://imported_replays");

        // keybinds config loading
        if (!FileAccess.FileExists("user://config.cfg")) {
            return;
        }
        
        var config = new ConfigFile();
        var err = config.Load("user://config.cfg");
        if (err != Error.Ok) {
            GD.Print("cant open config file");
        }

        foreach (var action in config.GetSectionKeys("Keybinds")) {
            string value = (string) config.GetValue("Keybinds", action);
            var bind = value[1..];
            InputMap.ActionEraseEvents(action);

            if (value.StartsWith('m')) {
                var mouseEvent = new InputEventMouseButton();
                mouseEvent.ButtonIndex = (Godot.MouseButton) Enum.Parse(typeof(Godot.MouseButton), bind);
                InputMap.ActionAddEvent(action, mouseEvent);
            } else if (value.StartsWith('k')) {
                var keyEvent = new InputEventKey();
                keyEvent.Keycode = (Godot.Key) Enum.Parse(typeof(Godot.Key), bind);
                InputMap.ActionAddEvent(action, keyEvent);
            } else if (bind == "None") {
                InputMap.ActionEraseEvents(action);
            }
        }
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
