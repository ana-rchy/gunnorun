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

    [Rpc] void Client_UpdatePuppetPositions(byte[] puppetPositionsSerialized) {
        var serializer = MessagePackSerializer.Get<Dictionary<long, Vector2>>();
        Dictionary<long, Vector2> puppetPositions = serializer.UnpackSingleObject(puppetPositionsSerialized);

        foreach (var kvp in puppetPositions) {
            GetNode<IPlayer>(Global.WORLD_PATH + kvp.Key).SetPuppetPosition(kvp.Value);
        }
    }
}