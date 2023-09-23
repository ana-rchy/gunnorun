using System;
using System.Collections.Generic;
using Godot;
using static Godot.GD;
using static Godot.MultiplayerApi;
using MsgPack.Serialization;

public partial class Client : Node {
	//---------------------------------------------------------------------------------//
	#region | rpc

	[Rpc(RpcMode.AnyPeer)] void Server_NewPlayerData(string username, Color color) {}

	[Rpc] void Client_Setup(byte[] serializedPlayerData) {
		var serializer = MessagePackSerializer.Get<Dictionary<long, Global.PlayerDataStruct>>();
		var playerData = serializer.UnpackSingleObject(serializedPlayerData);

		Global.OtherPlayerData = playerData;

		GetTree().ChangeSceneToFile("res://scenes/UI/Lobby.tscn");
		Rpc(nameof(Server_NewPlayerData), Global.PlayerData.Username, Global.PlayerData.Color);
	}

	[Rpc] void Client_NewPlayer(long id, string username, Color color) {
		if (Multiplayer.GetUniqueId() != id) {
			Global.OtherPlayerData.TryAdd(id, new Global.PlayerDataStruct(username, color));

			GetNode<Lobby>("/root/Lobby").RefreshList();
		}
	}

	[Rpc] void Client_PlayerLeft(long id, string gameState) {
		Global.OtherPlayerData.Remove(id);

		if (gameState == "Lobby") {
			GetNode<Lobby>("/root/Lobby").RefreshList();
		} else if (gameState == "Ingame") {
			GetNode(Global.WORLD_PATH + id).QueueFree();
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

	public void LeaveServer() {
		Multiplayer.MultiplayerPeer.Close();
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		GetTree().ChangeSceneToFile("res://scenes/UI/Menu.tscn");
	}

	#endregion
}
