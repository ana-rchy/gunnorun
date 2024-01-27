using System;
using Godot;

public partial class Inputs : Node {
    [Signal] public delegate void PausedEventHandler(bool paused);

    [Export] Timer _pauseTimer;

    public override void _Ready() {
        Paths.AddNodePath("INPUTS", GetPath());
        ProcessMode = ProcessModeEnum.Always;

        // keybinds config loading
        if (!FileAccess.FileExists("user://config.cfg")) return;
        
        ConfigFile config = new();
        var err = config.Load("user://config.cfg");
        if (err != Error.Ok) {
            GD.Print("cant open config file");
        }

        RefreshKeybinds(config);
    }

    bool _canPause = true;
    public override void _UnhandledInput(InputEvent e) {

        if (e.IsActionPressed("Leave")) {
			this.GetNodeConst<Client>("SERVER").LeaveServer();
            GetTree().Paused = false;
		} else if (e.IsActionPressed("Respawn") && Multiplayer.GetPeers().Length == 0) {
            GetTree().ChangeSceneToFile($"res://scenes/worlds/{Global.CurrentWorld}.tscn");
            GetTree().Paused = false;
        } else if (e.IsActionPressed("Pause") && Multiplayer.GetPeers().Length == 0
                   && this.GetNodeConst("WORLD") != null) {
            
            TogglePause();
        }
	}

    //---------------------------------------------------------------------------------//
    #region | funcs

    // side-effects
    void RefreshKeybinds(ConfigFile config) {
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

    void TogglePause() {
        if (!_canPause && GetTree().Paused == false) return;

        GetTree().Paused = !GetTree().Paused;
        EmitSignal(SignalName.Paused, GetTree().Paused);

        if (GetTree().Paused) {
            _canPause = false;
        } else {
            _pauseTimer.Start();
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnWorldLoaded() {
        _canPause = true;
    }

    void _OnPauseTimeout() {
        _canPause = true;
    }

    #endregion
}