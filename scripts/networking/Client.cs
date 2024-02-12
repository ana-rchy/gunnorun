using System;
using Godot;

public partial class Client : Node {
	[Export(PropertyHint.File)] string _menuScene;
	[Export] StateMachine _stateMachine;

	public override void _Ready() {
		Multiplayer.ServerDisconnected += _OnServerDisconnected;
	}

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
	#region | signals

	void _OnPeerDisconnected(long id) {
		Global.OtherPlayerData.Remove(id);
	}

	void _OnServerDisconnected() {
		LeaveServer();
		_stateMachine.ChangeState("InLobby");
		Global.PlayerData.ReadyStatus = false;
	}


	public void _OnJoinPressed(string ip, int port) {
		JoinServer(ip, port);
	}

	public void _OnFinishTimerTimeout() {
		LeaveServer();
	}

	#endregion
}
