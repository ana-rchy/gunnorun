using Godot;
using System;

public partial class Menu : Panel {
    const string WORLD_PATH = "res://scenes/worlds/";

    SceneTree Tree;
    Client Client;

    LineEdit UsernameField;
    ColorPickerButton ColorField;

    public override void _Ready() {
        Tree = GetTree();
        Client = GetNode<Client>(Global.SERVER_PATH);

        UsernameField = GetNode<LineEdit>("Username");
        ColorField = GetNode<ColorPickerButton>("PlayerColor");
        UsernameField.Text = Global.PlayerData.Username;
        ColorField.Color = Global.PlayerData.Color;
    } 

    //---------------------------------------------------------------------------------//
    #region | signals

    private void _OnSingleplayerPressed() {
        Tree.ChangeSceneToFile("res://scenes/worlds/" + Global.CurrentWorld + ".tscn");
        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;
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
