using Godot;
using static Godot.GD;
using static Godot.MultiplayerApi;
using System;
using System.Collections.Generic;
using MsgPack.Serialization;

public partial class Client : Node {
    public override void _Input(InputEvent e) {
        if (e.IsActionPressed("Leave")) {
            LeaveServer();
        }
    }

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer)] void Server_NewPlayerData(string username, Color color) {}

    [Rpc] void Client_Setup(byte[] serializedPlayerData, string gameState) {
        var playerDataSerializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
        Dictionary<long, Global.PlayerDataStruct> playerData = playerDataSerializer.UnpackSingleObject(serializedPlayerData);
        Global.OtherPlayerData = playerData;

        switch (gameState) {
            case "Lobby":
                GetTree().ChangeSceneToFile("res://scenes/UI/Lobby.tscn"); break;
            case "Ingame":
                GetTree().ChangeSceneToFile("res://scenes/worlds/AlphaArena.tscn"); break;
        }
        

        Rpc(nameof(Server_NewPlayerData), Global.PlayerData.Username, Global.PlayerData.Color);
    }

    [Rpc] void Client_NewPlayer(long id, string username, Color color, bool inLobby) {
        if (Multiplayer.GetUniqueId() != id) {
            Global.OtherPlayerData.TryAdd(id, new Global.PlayerDataStruct(username, color));
            
            if (inLobby) {
                GetNode<Lobby>("/root/Lobby").RefreshList();
            }
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void JoinServer(string ip, int port) {
        var peer = new ENetMultiplayerPeer();
        peer.CreateClient(ip, port);
        Multiplayer.MultiplayerPeer = peer;
    }

    private void LeaveServer() {
        Multiplayer.MultiplayerPeer.Close();
        GetTree().ChangeSceneToFile("res://scenes/UI/Menu.tscn");
    }

    #endregion
}