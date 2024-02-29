using Godot;
using System;

public partial class Settings : Node {
    public static Color CrosshairColor = new Color("#ffffff");

    public override void _Ready() {
        Paths.AddNodePath("SETTINGS", GetPath());

        if (!FileAccess.FileExists("user://config.cfg")) {
           (new ConfigFile()).Save("user://config.cfg");
        }

        ConfigFile config = new();
        var err = config.Load("user://config.cfg");
        if (err != Error.Ok) {
            GD.Print("cant open config file");
            return;
        }

        GetKeybinds(config);
        GetSettings(config);
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    // side-effects
    void GetKeybinds(ConfigFile config) {
        if (!config.HasSection("Keybinds")) return;

        foreach (var action in config.GetSectionKeys("Keybinds")) {
            string value = (string) config.GetValue("Keybinds", action);
            var bind = value[1..];
            InputMap.ActionEraseEvents(action);

            if (value.StartsWith('m')) {
                InputEventMouseButton mouseEvent = new();
                mouseEvent.ButtonIndex = (Godot.MouseButton) Enum.Parse(typeof(Godot.MouseButton), bind);
                InputMap.ActionAddEvent(action, mouseEvent);
            } else if (value.StartsWith('k')) {
                InputEventKey keyEvent = new();
                keyEvent.Keycode = (Godot.Key) Enum.Parse(typeof(Godot.Key), bind);
                InputMap.ActionAddEvent(action, keyEvent);
            } else if (bind == "None") {
                InputMap.ActionEraseEvents(action);
            }
        }
    }

    void GetSettings(ConfigFile config) {
        if (!config.HasSection("General")) return;

        CrosshairColor = (Color) config.GetValue("General", "CrosshairColor");
    }

    void SaveSettings() {
        ConfigFile config = new();

        config.SetValue("General", "CrosshairColor", CrosshairColor);
        
        config.Save("user://config.cfg");
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void _OnSettingsPanelClosed() {
        SaveSettings();
    }

    #endregion
}
