using Godot;
using static Godot.GD;
using static Godot.MultiplayerApi;
using System;
using System.Collections.Generic;
using MsgPack.Serialization;

public partial class Client : Node {
    public void JoinServer(string ip, int port) {
        var peer = new ENetMultiplayerPeer();
        peer.CreateClient(ip, port);
        Multiplayer.MultiplayerPeer = peer;
    }

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer)] void Server_PlayerData(string username, Color playerColor) {}

    [Rpc] void Client_Setup(string worldName, byte[] serializedData) {
        GetTree().ChangeSceneToFile("res://scenes/worlds/" + worldName + ".tscn");

        var serializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
        Dictionary<long, Global.PlayerDataStruct> playerData = serializer.UnpackSingleObject(serializedData);
        foreach (var kvp in playerData) { // create new puppetplayer and set player data
            GD.Print(kvp.Key);
            CallDeferred(nameof(CreateNewPlayer), kvp.Key, kvp.Value.Username, kvp.Value.Color);
        }

        Rpc("Server_PlayerData", Global.PlayerData.Username, Global.PlayerData.Color);
    }

    [Rpc] void Client_NewPlayer(long id, string username, Color playerColor) {
        if (Multiplayer.GetUniqueId() != id) {
            CreateNewPlayer(id, username, playerColor);
        }
    }

    [Rpc] void Client_PlayerDisconnected(long id) {
        GetNode(Global.WORLD_PATH + id.ToString()).QueueFree();
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    void CreateNewPlayer(long id, string username, Color playerColor) {
        var newPlayer = Load<PackedScene>("res://scenes/player/PuppetPlayer.tscn").Instantiate();
        GetNode(Global.WORLD_PATH).CallDeferred("add_child", newPlayer);

        newPlayer.Name = id.ToString();
        newPlayer.GetNode<Label>("Username").Text = username;

        ((ShaderMaterial) newPlayer.GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
    }

    #endregion
}