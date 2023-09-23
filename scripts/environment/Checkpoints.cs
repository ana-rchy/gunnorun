using System;
using System.Collections.Generic;
using Godot;

public partial class Checkpoints : Node {
    public override void _Ready() {
        if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer) {
            Global.UnpassedCheckpoints = new List<Node>(FindChildren("*", "Area2D"));

            foreach (Area2D checkpoint in Global.UnpassedCheckpoints) {
                checkpoint.BodyEntered += (Node2D body) => Global.UnpassedCheckpoints.Remove(checkpoint);
            }
        }
    }
}