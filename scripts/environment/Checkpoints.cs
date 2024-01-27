using System;
using System.Collections.Generic;
using Godot;

public partial class Checkpoints : Node {
    public static List<Node> UnpassedCheckpoints { get; private set; }
    
    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            QueueFree();
            return;
        }
        
        UnpassedCheckpoints = GetAllCheckpoints();

        foreach (Area2D checkpoint in UnpassedCheckpoints) {
            checkpoint.BodyEntered += (Node2D body) => UnpassedCheckpoints.Remove(checkpoint);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    // state-unpure (dependent on world)
    List<Node> GetAllCheckpoints() {
        return new List<Node>(FindChildren("*", "Area2D"));
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnLapPassed(int lapCount) {
        UnpassedCheckpoints = GetAllCheckpoints();
    }

    #endregion
}