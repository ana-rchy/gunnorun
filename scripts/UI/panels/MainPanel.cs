using System;
using Godot;

public partial class MainPanel : Panel {
    protected SceneTree Tree;
    protected Client Client;

    protected LineEdit UsernameField;
    protected ColorPickerButton ColorField;

    public override void _Ready() {
        Tree = GetTree();
        Client = GetNode<Client>(Global.SERVER_PATH);

        UsernameField = GetNode<LineEdit>("Username");
        ColorField = GetNode<ColorPickerButton>("PlayerColor");
        UsernameField.Text = Global.PlayerData.Username;
        ColorField.Color = Global.PlayerData.Color;

        Global.ReplayOnly = false;
        Global.DebugReplay = false;
    }
}