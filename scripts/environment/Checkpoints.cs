using System;
using System.Collections.Generic;
using Godot;

public partial class Checkpoints : Node {
    public static List<Node> UnpassedCheckpoints;
    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            QueueFree();
            return;
        }
        
        RefreshCheckpoints();

        foreach (Area2D checkpoint in UnpassedCheckpoints) {
            checkpoint.BodyEntered += (Node2D body) => UnpassedCheckpoints.Remove(checkpoint);
        }
    }

    public void RefreshCheckpoints() {
        UnpassedCheckpoints = new List<Node>(FindChildren("*", "Area2D"));
    }
}