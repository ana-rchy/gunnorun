using System;
using Godot;

public partial class Inputs : Node {
    [Signal] public delegate void PausedEventHandler(bool paused);

    [Export] Timer _pauseTimer;

    public override void _Ready() {
        Paths.AddNodePath("INPUTS", GetPath());
        ProcessMode = ProcessModeEnum.Always;
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
            if (!_canPause && GetTree().Paused == false) {
                return;
            }

            GetTree().Paused = !GetTree().Paused;
            EmitSignal(SignalName.Paused, GetTree().Paused);

            if (GetTree().Paused) {
                _canPause = false;
            } else {
                _pauseTimer.Start();
            }
        }
	}

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