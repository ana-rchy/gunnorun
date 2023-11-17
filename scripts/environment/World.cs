using Godot;
using System;

public partial class World : Node {
    [Signal] public delegate void WorldLoadedEventHandler();

    public override void _Ready() {
        WorldLoaded += this.GetNodeConst<Inputs>("INPUTS")._OnWorldLoaded;
        EmitSignal(SignalName.WorldLoaded);
    }
}