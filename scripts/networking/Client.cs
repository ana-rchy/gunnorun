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

    public void JoinServer(string ip, int port) {
        var peer = new ENetMultiplayerPeer();
        peer.CreateClient(ip, port);
        Multiplayer.MultiplayerPeer = peer;
    }

    private void LeaveServer() {
        Multiplayer.MultiplayerPeer.Close();
        GetTree().ChangeSceneToFile("res://scenes/Menu.tscn");
    }

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer)] void Server_PlayerData(string username, Color playerColor) {}

    [Rpc] void Client_Setup(byte[] serializedPlayerData, string gameState) {
        var playerDataSerializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
        Dictionary<long, Global.PlayerDataStruct> playerData = playerDataSerializer.UnpackSingleObject(serializedPlayerData);
        Global.OtherPlayerData = playerData;
        
        Rpc(nameof(Server_PlayerData), Global.PlayerData.Username, Global.PlayerData.Color);

        switch (gameState) {
            case "Lobby":
                GetTree().ChangeSceneToFile("res://scenes/Lobby.tscn"); break;
            case "Ingame":
                GetTree().ChangeSceneToFile("res://scenes/worlds/AlphaArena.tscn");

                var playerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
                foreach (var player in Global.OtherPlayerData) {
                    playerManager.CreateNewPuppetPlayer(player.Key, player.Value.Username, player.Value.Color);
                } break;
        }
        

        /*
        GetTree().ChangeSceneToFile("res://scenes/worlds/" + worldName + ".tscn");

        var serializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
        Dictionary<long, Global.PlayerDataStruct> playerData = serializer.UnpackSingleObject(serializedData);
        foreach (var kvp in playerData) { // create new puppetplayer and set player data
            CallDeferred(nameof(CreateNewPuppetPlayer), kvp.Key, kvp.Value.Username, kvp.Value.Color);
        }

        Rpc("Server_PlayerData", Global.PlayerData.Username, Global.PlayerData.Color);
        */
    }

    [Rpc] void Client_NewPlayer(long id, string username, Color playerColor) {
        if (Multiplayer.GetUniqueId() != id) {
            Global.OtherPlayerData.TryAdd(id, new Global.PlayerDataStruct(username, playerColor));

            switch (Global.GameState) {
                case "Lobby":
                    var lobby = GetNode<Lobby>("/root/Lobby");
                    lobby.RefreshList(); break;
                case "Ingame":
                    var playerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
                    playerManager.CreateNewPuppetPlayer(id, username, playerColor); break;
            }
            
            //CreateNewPuppetPlayer(id, username, playerColor);
        }
    }

    [Rpc] void Client_PlayerDisconnected(long id) {
        Global.OtherPlayerData.Remove(id);
        
        switch (Global.GameState) {
            case "Lobby":
                var lobby = GetNode<Lobby>("/root/Lobby");
                lobby.RefreshList(); break;
            case "Ingame":
                GetNode(Global.WORLD_PATH + id.ToString()).QueueFree(); break;
        }

        //GetNode(Global.WORLD_PATH + id.ToString()).QueueFree();
    }

    #endregion
}