using Godot;
using System;

public partial class World : Node {
    public override void _Ready() {
        GetNode<Timer>("Markers/FinishTimer").Timeout += GetNode<Client>(Global.SERVER_PATH)._OnFinishTimerTimeout;
    }
}