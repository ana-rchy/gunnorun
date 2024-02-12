using System;
using System.Collections.Generic;
using Godot;
using static Godot.MultiplayerApi;

public partial class Client : Node {
	[Export(PropertyHint.File)] string _menuScene;

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

	void _OnServerDisconnected() {
		LeaveServer();
	}

	public void _OnJoinPressed(string ip, int port) {
		JoinServer(ip, port);
	}

	public void _OnFinishTimerTimeout() {
		LeaveServer();
	}

	#endregion
}
