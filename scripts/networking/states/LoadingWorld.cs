using System;
using System.Collections.Generic;
using Godot;

public partial class LoadingWorld : State {
	public override void Enter(Dictionary<string, object> message) {
		GetTree().ChangeSceneToFile($"res://scenes/worlds/{message["world"]}.tscn");
	}

	public override void Update() {
		if (this.GetNodeConst("WORLD") != null) {
			StateMachine.ChangeState("StartingGame");
		}
	}
}