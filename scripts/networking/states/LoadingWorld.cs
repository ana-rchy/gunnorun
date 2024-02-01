using System;
using System.Collections.Generic;
using Godot;

public partial class LoadingWorld : State {
    [Export(PropertyHint.File)] string _puppetPlayerScene;

    public override void Enter(Dictionary<string, object> message) {
        GetTree().ChangeSceneToFile($"res://scenes/worlds/{message["world"]}.tscn");
    }

    public override void Update() {
        if (this.GetNodeConst("WORLD") != null) {
            AddPuppetPlayers();
            StateMachine.ChangeState("InGame");
        }
    }

    // side-effects
    void AddPuppetPlayers() {
        foreach (var player in Global.OtherPlayerData) {
            CreateNewPuppetPlayer(player.Key, player.Value.Username, player.Value.Color);
        }
    }

    void CreateNewPuppetPlayer(long id, string username, Color playerColor) {
        var newPlayer = GD.Load<PackedScene>(_puppetPlayerScene).Instantiate();
        this.GetNodeConst("WORLD").CallDeferred("add_child", newPlayer);

        newPlayer.Name = id.ToString();
        newPlayer.GetNode<Label>("Username").Text = username;
        ((ShaderMaterial) newPlayer.GetNode<AnimatedSprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
    }
}
