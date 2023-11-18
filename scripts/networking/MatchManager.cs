using Godot;
using System;

public partial class MatchManager : Node {
    [Export(PropertyHint.Dir)] string _worldDir;
    [Export] PlayerManager _playerManager;

    [Rpc] void Client_LoadWorld(string worldName) {
        Global.PlayerData.ReadyStatus = false;
        GetTree().ChangeSceneToFile($"{_worldDir}/{worldName}.tscn");

        foreach (var player in Global.OtherPlayerData) {
            _playerManager.CallDeferred("CreateNewPuppetPlayer", player.Key, player.Value.Username, player.Value.Color);
        }
    }

    [Rpc] void Client_PlayerWon(long id, double time) {
        var name = Multiplayer.GetUniqueId() == id ? Global.PlayerData.Username : Global.OtherPlayerData[id].Username;
        
        var playerUI = GetNode<PlayerUI>($"{Paths.GetNodePath("WORLD")}/{Multiplayer.GetUniqueId()}/PlayerUI");
        playerUI._OnRaceFinished((float) Math.Round(time, 3), name);
    }
}