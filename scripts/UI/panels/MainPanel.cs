using System;
using Godot;

public partial class MainPanel : Panel {
    [Export] protected LineEdit UsernameField;
    [Export] protected ColorPickerButton ColorField;
    [Export] protected LineEdit OpposingUsernameField;
    [Export] protected ColorPickerButton OpposingColorField;

    protected SceneTree Tree;
    protected Client Client;

    public override void _Ready() {
        Tree = GetTree();
        Client = this.GetNodeConst<Client>("SERVER");

        UsernameField.Text = Global.PlayerData.Username;
        ColorField.Color = Global.PlayerData.Color;

        Global.ReplayOnly = false;
        Global.DebugReplay = false;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnUsernameChanged(string text) {
        Global.PlayerData.Username = text;
        OpposingUsernameField.Text = text;
    }

    void _OnColorChanged(Color color) {
        Global.PlayerData.Color = color;
        OpposingColorField.Color = color;
    }

    #endregion
}