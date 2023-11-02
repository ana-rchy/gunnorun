using Godot;
using System;

public partial class MatchManager : Node {
    [Rpc] void Client_PlayerWon(long id, double time) {
        var name = Multiplayer.GetUniqueId() == id ? Global.PlayerData.Username : Global.OtherPlayerData[id].Username;
        
        var playerUI = GetNode<PlayerUI>(Global.WORLD_PATH + Multiplayer.GetUniqueId().ToString() + "/PlayerUI");
        playerUI._OnRaceFinished((float) Math.Round(time, 3));

        GetNode<Timer>(Global.WORLD_PATH + "Markers/FinishTimer").Start(); // needed so that timer doesnt continue after scene change
    }
}