using System;
using System.Collections.Generic;
using Godot;
using static Godot.MultiplayerApi;
using MsgPack.Serialization;

public partial class InLobby : State {
    [Export(PropertyHint.File)] string _lobbyScene;

    public override void _Ready() {
        Paths.AddNodePath("IN_LOBBY_STATE", GetPath());

        Multiplayer.PeerDisconnected += _OnPeerDisconnected;
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    // side-effects
    void UpdatePlayerStatus(long id, bool ready) {
        var player = Global.OtherPlayerData[id];
        player.ReadyStatus = ready;
        Global.OtherPlayerData[id] = player;
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | rpc
    
    [Rpc(RpcMode.AnyPeer)] void Server_NewPlayerData(string username, Color color) {}
    [Rpc(RpcMode.AnyPeer)] void Server_UpdateStatus(bool ready) {}

    [Rpc] void Client_Setup(byte[] serializedPlayerData) {
        var serializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
        var playerData = serializer.UnpackSingleObject(serializedPlayerData);

        Global.OtherPlayerData = playerData;

        GetTree().ChangeSceneToFile(_lobbyScene);
        Rpc(nameof(Server_NewPlayerData), Global.PlayerData.Username, Global.PlayerData.Color);
    }

    [Rpc] void Client_NewPlayer(long id, string username, Color color) {
        if (Multiplayer.GetUniqueId() != id) {
            Global.OtherPlayerData.TryAdd(id, new Global.PlayerDataStruct(username, color));

            this.GetNodeConst<Lobby>("LOBBY").RefreshList();
        }
    }


    [Rpc] void Client_UpdateStatus(long id, bool ready) {
        if (Multiplayer.GetUniqueId() != id) {
            UpdatePlayerStatus(id, ready);
            this.GetNodeConst<Lobby>("LOBBY").RefreshList();
        }
    }

    [Rpc] void Client_StartGame(string worldName) {
        StateMachine.ChangeState("LoadingWorld", new() {{ "world", worldName }} );
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnPeerDisconnected(long id) {
        this.GetNodeConst<Lobby>("LOBBY").RefreshList();
    }

    public void _OnReadyToggled(bool readyStatus) {
        if (!IsActiveState()) return;

        Rpc(nameof(Server_UpdateStatus), readyStatus);
    }

    #endregion
}
