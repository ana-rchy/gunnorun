using Godot;
using System;
using System.Linq;

public partial class Lap : Node {
    Checkpoints CheckpointManager;

    [Export] int MaxLaps;
    int LapCount = 0;

    public override void _Ready() {
        if (Multiplayer.GetPeers().Length != 0) {
            ProcessMode = ProcessModeEnum.Disabled;
        }

        CheckpointManager = GetNode<Checkpoints>("../Checkpoints");

        EmitSignal(SignalName.LapPassed, LapCount, MaxLaps);
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void LapPassedEventHandler(int lapCount, int maxLaps);
    [Signal] public delegate void RaceFinishedEventHandler();

    void _OnPlayerEntered(Node2D player) {
        if (Checkpoints.UnpassedCheckpoints.Count == 0) {
            if (LapCount < MaxLaps) {
                LapCount++;
            } else {
                EmitSignal(SignalName.RaceFinished, LevelTimer.Time);
                GetNode<Timer>("../FinishTimer").Start();

                // var levelTimer = player.GetNode<LevelTimer>("Timers/LevelTimer");
                // var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

                // var time = levelTimer.StopTimer();

                // extraUI.Show();
                // extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

                // player.GetNode<ReplayRecorder>("ReplayRecorder").StopRecording(time);

                // GetNode<Timer>("../FinishTimer").Start();
            }
        }

        EmitSignal(SignalName.LapPassed, LapCount, MaxLaps);

        // var lapCounter = GetNode<PlayerUI>(Global.WORLD_PATH + "Player/PlayerUI").LapCounter;
        // lapCounter.Text = "lap " + LapCount.ToString() + "/" + MaxLaps.ToString();
    }

    // void _OnTimeout() {
    //     GetNode<Client>(Global.SERVER_PATH).LeaveServer();
    // }

    #endregion
}