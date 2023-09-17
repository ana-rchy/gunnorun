using Godot;
using System;

public partial class Menu : Panel {
    const string WORLD_PATH = "res://scenes/worlds/";

    SceneTree Tree;
    Client Client;

    LineEdit UsernameField;
    ColorPickerButton ColorField;
    OptionButton MapSelect;
    Label LastTime;

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
        LastTime = GetNodeOrNull<Label>("LastTime");
        if (MapSelect != null) MapSelect.Selected = Global.SelectedWorldIndex;
        if (Global.LastTime != 0 && LastTime != null) LastTime.Text = "last time: " + Global.LastTime.ToString() + "s";
    } 

    //---------------------------------------------------------------------------------//
    #region | signals

    private void _OnSingleplayerPressed() {
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();

        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;
        Global.SelectedWorldIndex = MapSelect.Selected;

        Tree.ChangeSceneToFile("res://scenes/worlds/" + MapSelect.GetItemText(Global.SelectedWorldIndex) + ".tscn");
    }

    private void _OnJoinPressed() {
        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;

        var ip = GetNode<LineEdit>("IP").Text;
        ip = ip == "" ? "localhost" : ip; // localhost by default, entered ip otherwise
        var port = (int) GetNode<SpinBox>("Port").Value;

        Client.JoinServer(ip, port);
    }

    #endregion
}
