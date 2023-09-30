using Godot;
using System;
using System.Linq;

public partial class Lap : Node {
    Checkpoints CheckpointManager;

    [Export] public int MaxLaps;
    public int LapCount = 0;

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            QueueFree();
            return;
        }
        
        CheckpointManager = GetNode<Checkpoints>("../Checkpoints");
    }

    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            if (LapCount < MaxLaps) {
                LapCount++;
                CheckpointManager.RefreshCheckpoints();
            } else {
                var levelTimer = player.GetNode<LevelTimer>("LevelTimer");
                var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

                var time = levelTimer.StopTimer();

                extraUI.Show();
                extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

                player.GetNode<ReplayRecorder>("ReplayRecorder").StopRecording(time);

                GetNode<Timer>("Timer").Start();
            }
        }

        var lapCounter = GetNode<PlayerUI>(Global.WORLD_PATH + "Player/PlayerUI").LapCounter;
        lapCounter.Text = "lap " + LapCount.ToString() + "/" + MaxLaps.ToString();
    }

    void _OnTimeout() {
        GetNode<Client>(Global.SERVER_PATH).LeaveServer();
    }
}