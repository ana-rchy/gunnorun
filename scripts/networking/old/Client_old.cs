using System;
using System.Collections.Generic;
using Godot;
using static Godot.MultiplayerApi;
using MsgPack.Serialization;

public partial class Client_old : Node {
	[Export(PropertyHint.File)] string _menuScene;
	[Export(PropertyHint.File)] string _lobbyScene;

	//---------------------------------------------------------------------------------//
	#region | funcs

	public void LeaveServer() {
		Multiplayer.MultiplayerPeer.Close();
		GetTree().ChangeSceneToFile(_menuScene);
	}

	void JoinServer(string ip, int port) {
		var peer = new ENetMultiplayerPeer();
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;
	}

	#endregion

	//---------------------------------------------------------------------------------//
	#region | rpc

	[Rpc(RpcMode.AnyPeer)] void Server_NewPlayerData(string username, Color color) {}

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

	[Rpc] void Client_PlayerLeft(long id, string gameState) {
		Global.OtherPlayerData.Remove(id);

		if (gameState == "Lobby") {
			this.GetNodeConst<Lobby>("LOBBY").RefreshList();
		} else if (gameState == "Ingame") {
			GetNode($"{Paths.GetNodePath("WORLD")}/{id}").QueueFree();
		}
	}

	#endregion

	//---------------------------------------------------------------------------------//
	#region | signals

	public void _OnJoinPressed(string ip, int port) {
		JoinServer(ip, port);
	}

	public void _OnFinishTimerTimeout() {
		LeaveServer();
	}

	#endregion
}
