using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

public partial class MenuPanel : Panel {
    const string WORLD_PATH = "res://scenes/worlds/";

    SceneTree Tree;
    Client Client;

    LineEdit UsernameField;
    ColorPickerButton ColorField;
    OptionButton MapSelect;
    OptionButton ReplaySelect;
    Label LastTime;
    Label BestTime;

    public override void _Ready() {
        Tree = GetTree();
        Client = GetNode<Client>(Global.SERVER_PATH);

        // shared
        UsernameField = GetNode<LineEdit>("Username");
        ColorField = GetNode<ColorPickerButton>("PlayerColor");
        UsernameField.Text = Global.PlayerData.Username;
        ColorField.Color = Global.PlayerData.Color;

        // singleplayer
        MapSelect = GetNodeOrNull<OptionButton>("MapSelect");
        ReplaySelect = GetNodeOrNull<OptionButton>("ReplaySelect");
        LastTime = GetNodeOrNull<Label>("LastTime");
        BestTime = GetNodeOrNull<Label>("BestTime");

        if (MapSelect != null) MapSelect.Selected = Global.SelectedWorldIndex;
        if (ReplaySelect != null) CheckImportedReplays();
        if (Global.LastTime != 0 && LastTime != null) LastTime.Text = "last time: " + Global.LastTime.ToString() + "s";
        if (BestTime != null) UpdateBestTime();

        // etc
        Global.ReplayOnly = false;
        Global.DebugReplay = false;
    } 

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnSingleplayerPressed() {
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();

        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;
        Global.SelectedWorldIndex = MapSelect.Selected;

        Tree.ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
    }

    void _OnJoinPressed() {
        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;

        var ip = GetNode<LineEdit>("IP").Text;
        ip = ip == "" ? "localhost" : ip; // localhost by default, entered ip otherwise
        var port = (int) GetNode<SpinBox>("Port").Value;

        Client.JoinServer(ip, port);
    }
    
    void _OnMapSelected(int index) {
        Global.CurrentWorld = MapSelect.GetItemText(index);
        UpdateBestTime();
        CheckImportedReplays();
    }

    void _OnReplaySelected(int index) {
        if (index == 0) {
            Global.ReplayName = null;
            return;
        }

        Global.ReplayName = ReplaySelect.GetItemText(index);
    }

    void _OnViewReplayPressed() {
        Global.ReplayOnly = true;

        Tree.ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    void UpdateBestTime() {
        var timePath = "user://" + Global.CurrentWorld + "_time.gsd";
        if (FileAccess.FileExists(timePath)) {
            using var timeFile = FileAccess.Open(timePath, FileAccess.ModeFlags.Read);
            BestTime.Text = "best time: " + timeFile.GetDouble().ToString() + "s";
        } else {
            BestTime.Text = "";
        }
    }

    void CheckImportedReplays() {
        var allFiles = DirAccess.GetFilesAt("user://imported_replays/");
        var replayFiles = allFiles.Where( file => file.EndsWith(".grp") );

        var currentMapReplays = replayFiles.Where( replay => {
            var replayFile = FileAccess.Open("user://imported_replays/" + replay, FileAccess.ModeFlags.Read);
            var replayData = (Godot.Collections.Dictionary<string, Variant>) replayFile.GetVar();
            var worldName = replayData["World"];

            return (string) worldName == Global.CurrentWorld;
        });

        ReplaySelect.Clear();
        ReplaySelect.AddItem("best time");
        foreach (var replayName in currentMapReplays) {
            ReplaySelect.AddItem(replayName);
        }
    }

    #endregion
}
