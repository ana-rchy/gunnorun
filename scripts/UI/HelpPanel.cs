using Godot;
using System;

public partial class HelpPanel : Panel {
    public override void _Ready() {
        switch (OS.GetName()) {
            case "Linux":
                GetNode<Label>("UserDataPath").Text = "user data path: ~/.local/share/godot/app_userdata/gunnorun/";
                break;
            case "Windows":
                GetNode<Label>("UserDataPath").Text = "user data path: %APPDATA%\\Godot\\app_userdata\\gunnorun\\";
                break;
            case "macOS":
                GetNode<Label>("UserDataPath").Text = "user data path: ~/Library/Application Support/Godot/app_userdata/gunnorun/";
                break;
        }
    }

    public override void _Input(InputEvent e) {
        var mousePos = GetGlobalMousePosition();
        if (e.IsActionPressed("Shoot") &&
        (mousePos.X < Position.X || mousePos.X > Position.X + Size.X || mousePos.Y < Position.Y || mousePos.Y > Position.Y + Size.Y)) { // outside the panel
            GetParent<ColorRect>().Hide();
        }
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnSaveDebug() {
        using var debugFile = FileAccess.Open("user://replays/debug/debug_replay.gdr", FileAccess.ModeFlags.Write);
        debugFile.StoreVar(Global.LastDebugData);
    }

    void _OnViewDebug() {
        Global.DebugReplay = true;
        Global.ReplayOnly = true;

        GetTree().ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
    }

    #endregion
}