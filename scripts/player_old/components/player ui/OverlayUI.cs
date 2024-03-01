using Godot;
using System;

public partial class OverlayUI : Control {
    [Export] Label _label;

    public override void _Ready() {
        Paths.AddNodePath("OVERLAY_UI", GetPath());

        this.GetNodeConst<Inputs>("INPUTS").Paused += _OnPause;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnPause(bool paused) {
        if (paused) {
            Show();
            _label.Text = "paused";
        } else {
            Hide();
        }
    }

    public void _OnRaceFinished(float finishTime, string playerName) {
        Show();

        _label.Text = $"{Math.Round(finishTime, 3)}s";
        if (playerName != "") {
            _label.Text = $"{playerName} has won\n" + _label.Text;
        }
    }

    #endregion
}
