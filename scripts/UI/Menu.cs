using Godot;
using System;

public partial class Menu : Panel {
    const string WORLD_PATH = "res://scenes/worlds/";

    SceneTree Tree;
    Client Client;

    public override void _Ready() {
        Tree = GetTree();
        Client = GetNode<Client>(Global.SERVER_PATH);
    } 

    //---------------------------------------------------------------------------------//
    #region | signals

    private void _OnSingleplayerPressed() {
        Tree.ChangeSceneToFile("res://scenes/Lobby.tscn");
        Global.PlayerData.Username = GetNode<LineEdit>("Username").Text;
        Global.PlayerData.Color = GetNode<ColorPickerButton>("PlayerColor").Color;
    }

    private void _OnJoinPressed() {
        Global.PlayerData.Username = GetNode<LineEdit>("Username").Text;
        Global.PlayerData.Color = GetNode<ColorPickerButton>("PlayerColor").Color;

        var ip = GetNode<LineEdit>("IP").Text;
        ip = ip == "" ? "localhost" : ip;
        var port = (int) GetNode<SpinBox>("Port").Value;

        Client.JoinServer(ip, port);
    }

    #endregion
}
