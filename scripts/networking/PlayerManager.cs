using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;
using MsgPack.Serialization;

public partial class PlayerManager : Node {
    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer, TransferMode = TransferModeEnum.UnreliableOrdered)] void Server_UpdatePlayerPosition(Vector2 position) {}

    [Rpc(TransferMode = TransferModeEnum.UnreliableOrdered)] void Client_UpdatePuppetPositions(byte[] puppetPositionsSerialized) {
        var serializer = MessagePackSerializer.Get<Dictionary<long, Vector2>>();
        Dictionary<long, Vector2> puppetPositions = serializer.UnpackSingleObject(puppetPositionsSerialized);
        puppetPositions.Remove(Multiplayer.GetUniqueId());

        foreach (var kvp in puppetPositions) {
            GetNode<PuppetPlayer>(Global.WORLD_PATH + kvp.Key).PuppetPosition = kvp.Value;
        }

        Rpc(nameof(Server_UpdatePlayerPosition), GetNode<Node2D>(Global.WORLD_PATH + Multiplayer.GetUniqueId()).Position);
    }

    [Rpc] void Client_RemovePlayer(long id) {
        Global.OtherPlayerData.Remove(id);
        
        GetNode(Global.WORLD_PATH + id).QueueFree();
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void CreateNewPuppetPlayer(long id, string username, Color playerColor) {
        var newPlayer = Load<PackedScene>("res://scenes/player/PuppetPlayer.tscn").Instantiate();
        GetNode(Global.WORLD_PATH).CallDeferred("add_child", newPlayer);

        newPlayer.Name = id.ToString();
        newPlayer.GetNode<Label>("Username").Text = username;
        ((ShaderMaterial) newPlayer.GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
    }

    #endregion
}