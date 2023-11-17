using System;
using Godot;
using static Godot.MultiplayerApi;

public partial class LobbyManager : Node {
    [Export(PropertyHint.Dir)] string _worldsDir;
    [Export] PlayerManager _playerManager;

    public override void _Ready() {
        Paths.AddNodePath("LOBBY_MANAGER", GetPath());
    }

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer)] void Server_UpdateStatus(bool ready) {}

    [Rpc] void Client_StartGame(string worldName) {
        Global.PlayerData.ReadyStatus = false;
        GetTree().ChangeSceneToFile($"{_worldsDir}/{worldName}.tscn");

        foreach (var player in Global.OtherPlayerData) {
            _playerManager.CallDeferred("CreateNewPuppetPlayer", player.Key, player.Value.Username, player.Value.Color);
        }
    }

    [Rpc] void Client_UpdateStatus(long id, bool ready) {
        if (Multiplayer.GetUniqueId() != id) {
            var player = Global.OtherPlayerData[id];
            player.ReadyStatus = ready;
            Global.OtherPlayerData[id] = player;

            this.GetNodeConst<Lobby>("LOBBY").RefreshList();
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
