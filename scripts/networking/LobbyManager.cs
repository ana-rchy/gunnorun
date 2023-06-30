using Godot;
using System;
using static Godot.MultiplayerApi;

public partial class LobbyManager : Node {
    [Rpc(RpcMode.AnyPeer)] void Server_UpdateStatus(bool ready) {}

    [Rpc] void Client_UpdateStatus(long id, bool ready) {
        if (Multiplayer.GetUniqueId() != id) {
            var player = Global.OtherPlayerData[id];
            player.ReadyStatus = ready;
            Global.OtherPlayerData[id] = player;

            GetNode<Lobby>("/root/Lobby").RefreshList();
        }
    }

    [Rpc] void Client_StartGame() {
        Global.GameState = "Ingame";
        GetTree().ChangeSceneToFile("res://scenes/worlds/AlphaArena.tscn");

        var playerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
        foreach (var player in Global.OtherPlayerData) {
            playerManager.CallDeferred("CreateNewPuppetPlayer", player.Key, player.Value.Username, player.Value.Color);
        }
    }
}
