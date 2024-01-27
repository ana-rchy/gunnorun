using System;
using System.Collections.Generic;
using Godot;
using static Godot.MultiplayerApi;

public partial class InLobby : State {
	public override void _Ready() {
        Paths.AddNodePath("IN_LOBBY_STATE", GetPath());
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

    [Rpc(RpcMode.AnyPeer)] void Server_UpdateStatus(bool ready) {}

    [Rpc] void Client_UpdateStatus(long id, bool ready) {
		if (!IsActiveState()) return;

        if (Multiplayer.GetUniqueId() != id) {
            UpdatePlayerStatus(id, ready);
            this.GetNodeConst<Lobby>("LOBBY").RefreshList();
        }
    }

	[Rpc] void Client_StartGame(string worldName) {
        if (!IsActiveState()) return;
        
		StateMachine.ChangeState("LoadingWorld", new() {{ "world", worldName }} );
	}

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnReadyToggled(bool readyStatus) {
		if (!IsActiveState()) return;

        Rpc(nameof(Server_UpdateStatus), readyStatus);
    }

    #endregion
}