using System;
using System.Linq;
using Godot;

public partial class SingleplayerPanel : MainPanel {
    [Export(PropertyHint.Dir)] string _worldsDir;
    [Export] OptionButton _mapSelect;
    [Export] OptionButton _replaySelect;
    [Export] Label _lastTime;
    [Export] Label _bestTime;

    public static int SelectedWorldIndex { get; private set; } = 0;
    public static int SelectedReplayIndex { get; private set; } = 0;

    public override void _Ready() {
        base._Ready();

        _bestTime.Text = GetBestTimeText();
        UpdateImportedReplays();
        _mapSelect.Selected = SelectedWorldIndex;
        _replaySelect.Selected = SelectedReplayIndex;
        if (LevelTimer.Time != 0) {
            _lastTime.Text = $"last time: {Math.Round(LevelTimer.Time, 3)}s";
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    // pure
    string GetBestTimeText() {
        var timePath = $"user://{Global.CurrentWorld}_time.gsd";
        if (FileAccess.FileExists(timePath)) {
            using var timeFile = FileAccess.Open(timePath, FileAccess.ModeFlags.Read);
            return $"best time: {timeFile.GetDouble()}s";
        } else {
            return "";
        }
    }

    // side-effects
    void UpdateImportedReplays() {
        var allFiles = DirAccess.GetFilesAt("user://imported_replays/");
        var replayFiles = allFiles.Where( file => file.EndsWith(".grp") );

        var currentMapReplays = replayFiles.Where( replay => {
            var replayFile = FileAccess.Open($"user://imported_replays/{replay}", FileAccess.ModeFlags.Read);
            var replayData = (Godot.Collections.Dictionary<string, Variant>) replayFile.GetVar();
            var worldName = replayData["World"];

            return (string) worldName == Global.CurrentWorld;
        });

        _replaySelect.Clear();
        _replaySelect.AddItem("best time");
        foreach (var replayName in currentMapReplays) {
            _replaySelect.AddItem(replayName);
        }
    }

    #endregion

        //---------------------------------------------------------------------------------//
    #region | signals

    void _OnSingleplayerPressed() {    
        Tree.ChangeSceneToFile($"{_worldsDir}/{Global.CurrentWorld}.tscn");
    }

    void _OnMapSelected(int index) {
        SelectedWorldIndex = _mapSelect.Selected;
        Global.CurrentWorld = _mapSelect.GetItemText(index);

        _bestTime.Text = GetBestTimeText();
        UpdateImportedReplays();
    }

    void _OnReplaySelected(int index) {
        SelectedReplayIndex = index;
        
        if (index == 0) {
            Global.ReplayName = null;
            return;
        }

        Global.ReplayName = _replaySelect.GetItemText(index);
    }

    void _OnViewReplayPressed() {
        Global.ReplayOnly = true;

        Tree.ChangeSceneToFile($"{_worldsDir}/{Global.CurrentWorld}.tscn");
    }

    void _OnSaveLastReplayPressed() {
        UpdateImportedReplays();
    }

    #endregion
}