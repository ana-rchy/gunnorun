using System;
using System.Collections.Generic;
using Godot;
using static Godot.MultiplayerApi;
using MsgPack.Serialization;

public partial class Client : Node {
	[Export(PropertyHint.File)] string MenuScene;
	[Export(PropertyHint.File)] string LobbyScene;

	public override void _UnhandledInput(InputEvent e) {
		if (e.IsActionPressed("Leave")) {
			LeaveServer();
		}
	}

	//---------------------------------------------------------------------------------//
	#region | funcs

	void JoinServer(string ip, int port) {
		var peer = new ENetMultiplayerPeer();
		peer.CreateClient(ip, port);
		Multiplayer.MultiplayerPeer = peer;
	}

	void LeaveServer() {
		Multiplayer.MultiplayerPeer.Close();
		GetTree().ChangeSceneToFile(MenuScene);
	}

	#endregion

	//---------------------------------------------------------------------------------//
	#region | rpc

	[Rpc(RpcMode.AnyPeer)] void Server_NewPlayerData(string username, Color color) {}

	[Rpc] void Client_Setup(byte[] serializedPlayerData) {
		var serializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
		var playerData = serializer.UnpackSingleObject(serializedPlayerData);

		Global.OtherPlayerData = playerData;

		GetTree().ChangeSceneToFile(LobbyScene);
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
			GetNode($"{Paths.GetNodePath("SERVER")}/{id}").QueueFree();
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
