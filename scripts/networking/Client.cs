using Godot;
using System;

public partial class Client : Node {
    public void JoinServer(string ip, int port) {
        var peer = new ENetMultiplayerPeer();
        peer.CreateClient(ip, port);
        Multiplayer.MultiplayerPeer = peer;
        GetTree().ChangeSceneToFile("res://scenes/worlds/Cave.tscn");
    }
}