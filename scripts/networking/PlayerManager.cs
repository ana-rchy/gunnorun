using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;
using MsgPack.Serialization;

public partial class PlayerManager : Node {
    const float MURASAMA_INTANGIBILITY_TIME = 0.3f;

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void CreateNewPuppetPlayer(long id, string username, Color playerColor) {
        var newPlayer = Load<PackedScene>("res://scenes/player/PuppetPlayer.tscn").Instantiate();
        GetNode(Global.WORLD_PATH).CallDeferred("add_child", newPlayer);

        newPlayer.Name = id.ToString();
        newPlayer.GetNode<Label>("Username").Text = username;
        ((ShaderMaterial) newPlayer.GetNode<AnimatedSprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer, TransferMode = TransferModeEnum.UnreliableOrdered)] void Server_UpdatePlayerPosition(Vector2 position) {}
    [Rpc(RpcMode.AnyPeer)] void Server_TracerShot(float rotation, float range) {}
    [Rpc(RpcMode.AnyPeer)] void Server_Intangibility(long id, float time) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerHPChanged(long id, int newHP) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerFrameChanged(byte frame) {}

    [Rpc(TransferMode = TransferModeEnum.UnreliableOrdered)] void Client_UpdatePuppetPositions(byte[] puppetPositionsSerialized) {
        var serializer = MessagePackSerializer.Get<Dictionary<long, Vector2>>();
        Dictionary<long, Vector2> puppetPositions = serializer.UnpackSingleObject(puppetPositionsSerialized);
        puppetPositions.Remove(Multiplayer.GetUniqueId());

        foreach (var kvp in puppetPositions) {
            var puppetPlayer = GetNodeOrNull<PuppetPlayer>(Global.WORLD_PATH + kvp.Key);
            if (puppetPlayer != null) {
                puppetPlayer.PuppetPosition = kvp.Value;
            }
        }

        var player = GetNodeOrNull<Node2D>(Global.WORLD_PATH + Multiplayer.GetUniqueId());
        if (player != null) {
            Rpc(nameof(Server_UpdatePlayerPosition), player.Position);
        }
    }

    [Rpc] void Client_TracerShot(long id, float rotation, float range) {
        GetNode<PuppetPlayer>(Global.WORLD_PATH + id.ToString()).SpawnTracer(rotation, range);

        // var tracerScene = Load<PackedScene>("res://scenes/player/Tracer.tscn");
        // var tracer = tracerScene.Instantiate<Tracer>();

        // tracer.GlobalPosition = GetNode<Node2D>(Global.WORLD_PATH + id.ToString()).GlobalPosition;
        // tracer.Rotation = rotation;
        // tracer.Range = range;

        // var tracerArea = tracer.GetNode<Area2D>("Area2D");
        // tracerArea.SetCollisionMaskValue(4, false);
        // tracerArea.SetCollisionMaskValue(2, true);

        // GetNode(Global.WORLD_PATH).AddChild(tracer);
    }

    [Rpc] void Client_Intangibility(float time) {
        var player = GetNode<Player>(Global.WORLD_PATH + Multiplayer.GetUniqueId().ToString());
        Task.Run(() => {
            _ = player.Intangibility(time);
        });
    }

    [Rpc] void Client_PlayerHPChanged(long id, int newHP) { 
        var player = GetNode<IPlayer>(Global.WORLD_PATH + id);
        player.ChangeHP(newHP, false);
    }

    [Rpc] void Client_PlayerFrameChanged(long id, int frame) {
        if (id != Multiplayer.GetUniqueId()) {
            var playerSprite = GetNode<AnimatedSprite2D>(Global.WORLD_PATH + id.ToString() + "/Sprite");

            playerSprite.Frame = frame;
        }
    }

    [Rpc] void Client_LapChanged(int lap, int maxLaps) {
        var lapManager = GetNode<Lap>(Global.WORLD_PATH + "Markers/Lap");
        lapManager.EmitSignal(Lap.SignalName.LapPassed, lap, maxLaps);

        // var lapCounter = GetNode<PlayerUI>(Global.WORLD_PATH + Multiplayer.GetUniqueId() + "/PlayerUI").LapCounter;
        // lapCounter.Text = "lap " + lap.ToString() + "/" + maxLaps.ToString();
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnWeaponShot(Player player) {
        var playerPosToMousePos = player.GlobalPosition.DirectionTo(player.GetGlobalMousePosition());
        Rpc(nameof(Server_TracerShot), new Vector2(0, 0).AngleToPoint(playerPosToMousePos), player.CurrentWeapon.Range);
    }

    public void _OnHPChanged(int newHP) {
        Rpc(nameof(Server_PlayerHPChanged), Multiplayer.GetUniqueId(), newHP);
    }

    public void _OnOtherPlayerHit(long playerID, int newHP, string weaponName) {
        Rpc(nameof(Server_PlayerHPChanged), playerID, newHP);

        if (weaponName == "Murasama") {
            Rpc(nameof(Server_Intangibility), playerID, Murasama.INTANGIBILITY_TIME);
        }
    }

    public void _OnPlayerFrameChanged(int frame) {
        Rpc(nameof(Server_PlayerFrameChanged), frame);
    }

    #endregion
}