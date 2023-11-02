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
        if (Input.IsMouseButtonPressed(MouseButton.Left) &&
            (mousePos.X < Position.X || mousePos.X > Position.X + Size.X || mousePos.Y < Position.Y || mousePos.Y > Position.Y + Size.Y)) { // outside the panel
            GetParent<ColorRect>().Hide();
        }
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnSaveDebug() {
        using var debugFile = FileAccess.Open("user://replays/debug/debug_replay.gdr", FileAccess.ModeFlags.Write);
        debugFile.StoreVar(DebugRecorder.LastDebugData);
    }

    void _OnViewDebug() {
        Global.DebugReplay = true;
        Global.ReplayOnly = true;

        GetTree().ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
    }

    void _OnClearData() {
        GetTree().Root.GuiEmbedSubwindows = false;

        var dialog = new ConfirmationDialog();
        dialog.DialogText = "are you sure?";
        GetTree().Root.AddChild(dialog);

        dialog.PopupCentered();

        dialog.Confirmed += () => {
            DirAccess.RemoveAbsolute("user://replays/debug/debug_replay.gdr");
            foreach (var file in DirAccess.GetFilesAt("user://replays")) {
                DirAccess.RemoveAbsolute("user://replays/" + file);
            }
            foreach (var file in DirAccess.GetFilesAt("user://imported_replays")) {
                DirAccess.RemoveAbsolute("user://imported_replays/" + file);
            }
            foreach (var file in DirAccess.GetFilesAt("user://")) {
                DirAccess.RemoveAbsolute("user://" + file);
            }
        };

        dialog.Confirmed += () => dialog.QueueFree();
        dialog.Canceled += () => dialog.QueueFree();
        dialog.TreeExited += () => GetTree().Root.GuiEmbedSubwindows = true;
    }

    void _OnVisibilityChanged() {
        GetNode<Label>("Controls").Text =
            InputMap.ActionGetEvents("Num1")[0].AsText() + "-" + InputMap.ActionGetEvents("Num4")[0].AsText() + " - switch weapons\n" +
            InputMap.ActionGetEvents("Reload")[0].AsText() + " - reload\n"+ 
            InputMap.ActionGetEvents("Leave")[0].AsText() + " - return to menu    " +
            InputMap.ActionGetEvents("Respawn")[0].AsText() +  "- respawn";
    }

    #endregion
}