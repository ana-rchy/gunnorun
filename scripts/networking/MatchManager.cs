using Godot;
using System;

public partial class MatchManager : Node {
    [Rpc] void Client_PlayerWon(long id) {
        string name;
        if (id == Multiplayer.GetUniqueId()) {
            name = Global.PlayerData.Username;
        } else {
            name = Global.OtherPlayerData[id].Username;
        }
        
        CanvasLayer extraUI = GetNode<CanvasLayer>(Global.WORLD_PATH + "ExtraUI");

        extraUI.GetNode<Label>("WinText").Text = name + " has won";
        extraUI.Show();
    }
}