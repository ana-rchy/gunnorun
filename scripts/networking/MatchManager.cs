using Godot;
using System;

public partial class MatchManager : Node {
    [Rpc] async void Client_PlayerWon(long id, double time) {
        string name;
        if (id == Multiplayer.GetUniqueId()) {
            name = Global.PlayerData.Username;
        } else {
            name = Global.OtherPlayerData[id].Username;
        }
        
        var extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

        time = Math.Round(time, 3);
        extraUI.GetNode<Label>("Label").Text = name + " has won\n" + time.ToString() + "s";
        extraUI.Show();

        await this.Sleep(3f);
        GetNode<Client>(Global.SERVER_PATH).LeaveServer();
    }
}