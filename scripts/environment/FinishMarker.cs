using System;
using Godot;

public partial class FinishMarker : Node {
    void _OnPlayerEntered(Node2D player) {
        if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer && Global.UnpassedCheckpoints.Count == 0) {
            var levelTimer = player.GetNode<LevelTimer>("LevelTimer");
            var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

            var time = levelTimer.StopTimer();

            extraUI.Show();
            extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

            player.GetNode<ReplayRecorder>("ReplayRecorder").StopRecording(time);

            GetNode<Timer>("Timer").Start();
        }
    }

    void _OnTimeout() { // needed instead of this.Sleep(), so that the timer doesnt continue on after scene changes, and change scene again
        GetNode<Client>(Global.SERVER_PATH).LeaveServer(); // function also used by MatchManager, so LeaveServer is used
    }
}