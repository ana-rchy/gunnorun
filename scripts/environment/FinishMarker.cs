using Godot;
using System;

public partial class FinishMarker : Node {
    private async void _OnPlayerEntered(Node2D player) {
        if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer) {
            CanvasLayer extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");
            LevelTimer levelTimer = GetNode<LevelTimer>(Global.WORLD_PATH + "LevelTimer");

            levelTimer.StopTimer();

            extraUI.Show();
            double time = Math.Round(levelTimer.Time, 3);
            extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

            await this.Sleep(3f);
            GetTree().ChangeSceneToFile("res://scenes/UI/Menu.tscn");
        }
    }
}