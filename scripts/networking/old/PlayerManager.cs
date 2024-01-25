using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;
using MsgPack.Serialization;

public partial class PlayerManager : Node {
    [Export(PropertyHint.File)] string _puppetPlayerScene;

    const float MURASAMA_INTANGIBILITY_TIME = 0.3f;

    //---------------------------------------------------------------------------------//
    #region | funcs

    void CreateNewPuppetPlayer(long id, string username, Color playerColor) {
        var newPlayer = GD.Load<PackedScene>(_puppetPlayerScene).Instantiate();
        GD.Print(this.GetNodeConst("WORLD"));
        this.GetNodeConst("WORLD").CallDeferred("add_child", newPlayer);

        newPlayer.Name = id.ToString();
        newPlayer.GetNode<Label>("Username").Text = username;
        ((ShaderMaterial) newPlayer.GetNode<AnimatedSprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer, TransferMode = TransferModeEnum.UnreliableOrdered)] void Server_UpdatePlayerPosition(Vector2 position) {}
    [Rpc(RpcMode.AnyPeer)] void Server_WeaponShot(string name, float rotation, float range) {}
    [Rpc(RpcMode.AnyPeer)] void Server_Intangibility(long id, float time) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerHPChanged(long id, int newHP) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerFrameChanged(byte frame) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerOnGround(bool onGround, float xVel) {}

    [Rpc(TransferMode = TransferModeEnum.UnreliableOrdered)] void Client_UpdatePuppetPositions(byte[] puppetPositionsSerialized) {
        var serializer = MessagePackSerializer.Get<Dictionary<long, Vector2>>();
        Dictionary<long, Vector2> puppetPositions = serializer.UnpackSingleObject(puppetPositionsSerialized);
        puppetPositions.Remove(Multiplayer.GetUniqueId());

        foreach (var kvp in puppetPositions) {
            var puppetPlayer = GetNodeOrNull<PuppetPlayer>($"{Paths.GetNodePath("WORLD")}/{kvp.Key}");
            if (puppetPlayer != null) {
                puppetPlayer.PuppetPosition = kvp.Value;
            }
        }

        var player = GetNodeOrNull<Node2D>($"{Paths.GetNodePath("WORLD")}/{Multiplayer.GetUniqueId()}");
        if (player != null) {
            Rpc(nameof(Server_UpdatePlayerPosition), player.Position);
        }
    }

    [Rpc] void Client_WeaponShot(long id, string name, float rotation, float range) {
        if (id == Multiplayer.GetUniqueId()) {
            return;
        }

        var puppetPlayer = GetNode<PuppetPlayer>($"{Paths.GetNodePath("WORLD")}/{id}");
        switch (name) {
            case "Murasama":
                puppetPlayer.GetNode<ParticlesManager>("Particles").EmitMurasamaParticles();
                break;
            default:
                puppetPlayer.SpawnTracer(rotation, range);
                break;
        }
    }

    [Rpc] void Client_Intangibility(float time) {
        var player = GetNode<Player>($"{Paths.GetNodePath("WORLD")}/{Multiplayer.GetUniqueId()}");
        Task.Run(() => {
            _ = player.Intangibility(time);
        });
    }

    [Rpc] void Client_PlayerHPChanged(long id, int newHP) {
        var player = GetNode<IPlayer>($"{Paths.GetNodePath("WORLD")}/{id}");
        player.ChangeHP(newHP);
    }

    [Rpc] void Client_PlayerFrameChanged(long id, int frame) {
        if (id == Multiplayer.GetUniqueId()) {
            return;
        }

        var playerSprite = GetNode<AnimatedSprite2D>($"{Paths.GetNodePath("WORLD")}/{id}/Sprite");

        playerSprite.Frame = frame;
    }

    [Rpc] void Client_PlayerOnGround(long id, bool onGround, float xVel) {
        if (id == Multiplayer.GetUniqueId()) {
            return;
        }

        var puppetPlayer = GetNode<PuppetPlayer>($"{Paths.GetNodePath("WORLD")}/{id}");
        puppetPlayer.EmitSignal(PuppetPlayer.SignalName.OnGround, onGround, xVel);
    }

    [Rpc] void Client_LapChanged(int lap, int maxLaps) {
        var lapManager = this.GetNodeConst<Lap>("LAP");
        lapManager.EmitSignal(Lap.SignalName.LapPassed, lap, maxLaps);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnWeaponShot(Player player) {
        var playerPosToMousePos = player.GlobalPosition.DirectionTo(player.GetGlobalMousePosition());
        Rpc(nameof(Server_WeaponShot), player.CurrentWeapon.Name,
            new Vector2(0, 0).AngleToPoint(playerPosToMousePos), player.CurrentWeapon.Range);
    }

    public void _OnOtherPlayerHit(long playerID, int newHP, string weaponName) {
        Rpc(nameof(Server_PlayerHPChanged), playerID, newHP);

        if (weaponName == "Murasama") {
            Rpc(nameof(Server_Intangibility), playerID, Murasama.INTANGIBILITY_TIME);
        }
    }

    public void _OnHPChanged(int newHP) {
        Rpc(nameof(Server_PlayerHPChanged), Multiplayer.GetUniqueId(), newHP);
    }

    public void _OnPlayerFrameChanged(int frame) {
        Rpc(nameof(Server_PlayerFrameChanged), frame);
    }

    public void _OnGround(bool onGround, float xVel) {
        Rpc(nameof(Server_PlayerOnGround), onGround, xVel);
    }

    #endregion
}