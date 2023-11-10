using Godot;
using System;

public partial class MatchManager : Node {
    [Rpc] void Client_PlayerWon(long id, double time) {
        var name = Multiplayer.GetUniqueId() == id ? Global.PlayerData.Username : Global.OtherPlayerData[id].Username;
        
        var playerUI = GetNode<PlayerUI>($"{Paths.GetNodePath("WORLD")}/{Multiplayer.GetUniqueId()}/PlayerUI");
        playerUI._OnRaceFinished((float) Math.Round(time, 3));

        this.GetNodeConst<Timer>("FINISH_TIMER").Start(); // needed so that timer doesnt continue after scene change
    }
}