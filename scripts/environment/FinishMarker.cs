using System;
using Godot;

public partial class FinishMarker : Node {
    [Signal] public delegate void RaceFinishedEventHandler();
    
    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            EmitSignal(SignalName.RaceFinished, LevelTimer.Time);
            GetNode<Timer>("../FinishTimer").Start();

            // var levelTimer = player.GetNode<LevelTimer>("Timers/LevelTimer");
            // var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

            // var time = levelTimer.StopTimer();

            // extraUI.Show();
            // extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

            // player.GetNode<ReplayRecorder>("ReplayRecorder").StopRecording(time);
        }
    }

    // void _OnTimeout() { // needed instead of this.Sleep(), so that the timer doesnt continue on after scene changes, and change scene again
    //     GetNode<Client>(Global.SERVER_PATH).LeaveServer(); // function also used by MatchManager, so LeaveServer is used
    // }
}