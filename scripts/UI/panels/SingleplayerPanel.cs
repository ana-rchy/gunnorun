using System;
using System.Linq;
using Godot;

public partial class SingleplayerPanel : MainPanel {
    OptionButton MapSelect;
    OptionButton ReplaySelect;
    Label LastTime;
    Label BestTime;

    public override void _Ready() {
        base._Ready();

        MapSelect = GetNode<OptionButton>("MapSelect");
        ReplaySelect = GetNode<OptionButton>("ReplaySelect");
        LastTime = GetNode<Label>("LastTime");
        BestTime = GetNode<Label>("BestTime");

        UpdateBestTime();
        CheckImportedReplays();
        MapSelect.Selected = MenuChoicePersistence.SelectedWorldIndex;
        ReplaySelect.Selected = MenuChoicePersistence.SelectedReplayIndex;
        if (Global.LastTime != 0) LastTime.Text = "last time: " + Global.LastTime.ToString() + "s";
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnSingleplayerPressed() {
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();        
        Tree.ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
    }

    void _OnMapSelected(int index) {
        MenuChoicePersistence.SelectedWorldIndex = MapSelect.Selected;
        Global.CurrentWorld = MapSelect.GetItemText(index);

        UpdateBestTime();
        CheckImportedReplays();
    }

    void _OnReplaySelected(int index) {
        MenuChoicePersistence.SelectedReplayIndex = index;
        
        if (index == 0) {
            Global.ReplayName = null;
            return;
        }

        Global.ReplayName = ReplaySelect.GetItemText(index);
    }

    void _OnUsernameChanged(string text) {
        Global.PlayerData.Username = text;

        GetNode<LineEdit>("/root/Menu/TabContainer/Multiplayer/Panel/Username").Text = text;
    }

    void _OnViewReplayPressed() {
        Global.ReplayOnly = true;

        Tree.ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
    }

    void _OnColorChanged(Color color) {
        Global.PlayerData.Color = color;

        GetNode<ColorPickerButton>("/root/Menu/TabContainer/Multiplayer/Panel/PlayerColor").Color = color;
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