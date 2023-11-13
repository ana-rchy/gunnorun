using System;
using System.Linq;
using Godot;

public partial class SingleplayerPanel : MainPanel {
    [Export(PropertyHint.Dir)] string WorldsDir;
    [Export] OptionButton MapSelect;
    [Export] OptionButton ReplaySelect;
    [Export] Label LastTime;
    [Export] Label BestTime;

    public static int SelectedWorldIndex { get; private set; } = 0;
    public static int SelectedReplayIndex { get; private set; } = 0;

    public override void _Ready() {
        base._Ready();

        UpdateBestTime();
        CheckImportedReplays();
        MapSelect.Selected = SelectedWorldIndex;
        ReplaySelect.Selected = SelectedReplayIndex;
        if (LevelTimer.Time != 0) {
            LastTime.Text = $"last time: {Math.Round(LevelTimer.Time, 3)}s";
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    void UpdateBestTime() {
        var timePath = $"user://{Global.CurrentWorld}_time.gsd";
        if (FileAccess.FileExists(timePath)) {
            using var timeFile = FileAccess.Open(timePath, FileAccess.ModeFlags.Read);
            BestTime.Text = $"best time: {timeFile.GetDouble()}s";
        } else {
            BestTime.Text = "";
        }
    }

    void CheckImportedReplays() {
        var allFiles = DirAccess.GetFilesAt("user://imported_replays/");
        var replayFiles = allFiles.Where( file => file.EndsWith(".grp") );

        var currentMapReplays = replayFiles.Where( replay => {
            var replayFile = FileAccess.Open($"user://imported_replays/{replay}", FileAccess.ModeFlags.Read);
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

        //---------------------------------------------------------------------------------//
    #region | signals

    void _OnSingleplayerPressed() {    
        Tree.ChangeSceneToFile($"{WorldsDir}/{Global.CurrentWorld}.tscn");
    }

    void _OnMapSelected(int index) {
        SelectedWorldIndex = MapSelect.Selected;
        Global.CurrentWorld = MapSelect.GetItemText(index);

        UpdateBestTime();
        CheckImportedReplays();
    }

    void _OnReplaySelected(int index) {
        SelectedReplayIndex = index;
        
        if (index == 0) {
            Global.ReplayName = null;
            return;
        }

        Global.ReplayName = ReplaySelect.GetItemText(index);
    }

    void _OnViewReplayPressed() {
        Global.ReplayOnly = true;

        Tree.ChangeSceneToFile($"{WorldsDir}/{Global.CurrentWorld}.tscn");
    }

    void _OnSaveLastReplayPressed() {
        CheckImportedReplays();
    }

    #endregion
}