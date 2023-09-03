using Godot;
using System;

public partial class FinishMarker : Node {
    private async void _OnPlayerEntered(Node2D player) {
        if (Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer) {
            var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");
            var levelTimer = player.GetNode<LevelTimer>("LevelTimer");

            var time = levelTimer.StopTimer();

            extraUI.Show();
            extraUI.GetNode<Label>("Label").Text = time.ToString() + "s";

            await this.Sleep(3f);
            GetTree().ChangeSceneToFile("res://scenes/UI/Menu.tscn");
        }
    }
}