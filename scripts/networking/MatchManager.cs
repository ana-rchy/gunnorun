using Godot;
using System;

public partial class MatchManager : Node {
    [Rpc] void Client_PlayerWon(long id, double time) {
        string name;
        if (Multiplayer.GetUniqueId() == id) {
            name = Global.PlayerData.Username;
        } else {
            name = Global.OtherPlayerData[id].Username;
        }
        
        var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

        time = Math.Round(time, 3);
        extraUI.GetNode<Label>("Label").Text = name + " has won\n" + time.ToString() + "s";
        extraUI.Show();

        GetNode<Timer>(Global.WORLD_PATH + "Markers/FinishTimer").Start(); // needed so that timer doesnt continue after scene change
    }
}