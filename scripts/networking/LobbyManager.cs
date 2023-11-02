using System;
using Godot;
using static Godot.MultiplayerApi;

public partial class LobbyManager : Node {
    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer)] void Server_UpdateStatus(bool ready) {}

    [Rpc] void Client_UpdateStatus(long id, bool ready) {
        if (Multiplayer.GetUniqueId() != id) {
            var player = Global.OtherPlayerData[id];
            player.ReadyStatus = ready;
            Global.OtherPlayerData[id] = player;

            GetNode<Lobby>("/root/Lobby").RefreshList();
        }
    }

    [Rpc] void Client_StartGame(string worldName) {
        Global.PlayerData.ReadyStatus = false;
        GetTree().ChangeSceneToFile("res://scenes/worlds/" + worldName + ".tscn");

        foreach (var player in Global.OtherPlayerData) {
            GetNode<PlayerManager>("../PlayerManager").CallDeferred("CreateNewPuppetPlayer", player.Key, player.Value.Username, player.Value.Color);
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnReadyToggled(bool readyStatus) {
        Rpc(nameof(Server_UpdateStatus), readyStatus);
    }

    #endregion
}
