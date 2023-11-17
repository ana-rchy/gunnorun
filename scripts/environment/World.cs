using Godot;
using System;

public partial class World : Node {
    [Signal] public delegate void WorldLoadedEventHandler();

    [Export] Timer _finishTimer;

    public override void _Ready() {
        Paths.AddNodePath("FINISH_TIMER", _finishTimer.GetPath());
        
        WorldLoaded += this.GetNodeConst<Inputs>("INPUTS")._OnWorldLoaded;
        if (Multiplayer.GetPeers().Length == 0) {
            _finishTimer.Timeout += this.GetNodeConst<Client>("SERVER")._OnFinishTimerTimeout;
        }

        EmitSignal(SignalName.WorldLoaded);
    }
}