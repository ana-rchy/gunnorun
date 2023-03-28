using Godot;
using System;
using System.Collections.Generic;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;
using MsgPack.Serialization;

public partial class PlayerManager : Node {
    [Rpc(RpcMode.AnyPeer, TransferMode = TransferModeEnum.UnreliableOrdered)] void Server_Shoot(Vector2 velocityDirection){}
    [Rpc(RpcMode.AnyPeer)] void Server_WeaponSwitch(int currentWeaponIndex){}
    [Rpc(RpcMode.AnyPeer)] void Server_Reload() {}

    [Rpc(TransferMode = TransferModeEnum.UnreliableOrdered)] void Client_UpdatePuppetPositions(byte[] puppetPositionsSerialized) {
        var serializer = MessagePackSerializer.Get<Dictionary<long, Vector2>>();
        Dictionary<long, Vector2> puppetPositions = serializer.UnpackSingleObject(puppetPositionsSerialized);
        puppetPositions.Remove(Multiplayer.GetUniqueId());

        foreach (var kvp in puppetPositions) {
            GetNode<PuppetPlayer>(Global.WORLD_PATH + kvp.Key).SetPuppetPosition(kvp.Value);
        }
    }

    [Rpc(TransferMode = TransferModeEnum.UnreliableOrdered)] void Client_ReconciliatePlayer(Vector2 position, Vector2 velocity) {
        var selfPlayer = GetNode<Player>(Global.WORLD_PATH + Multiplayer.GetUniqueId().ToString());

        selfPlayer.ReconciliateWithServer(position, velocity);
    }
}