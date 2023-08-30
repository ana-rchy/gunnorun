using Godot;
using System;

public partial class FinishMarker : Node {
    private async void _OnPlayerEntered(Node2D player) {
        if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer) {
            var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");
            var levelTimer = GetNode<LevelTimer>(Global.WORLD_PATH + "LevelTimer");

            levelTimer.StopTimer();

            extraUI.Show();
            var time = Math.Round(levelTimer.Time, 3);
            extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

            await this.Sleep(3f);
            GetTree().ChangeSceneToFile("res://scenes/UI/Menu.tscn");
        }
    }
}