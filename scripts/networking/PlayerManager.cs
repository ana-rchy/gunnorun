using Godot;
using System;
using System.Collections.Generic;
using static Godot.MultiplayerApi;

public partial class PlayerManager : Node {
    [Rpc(RpcMode.AnyPeer)] void Server_Shoot(Vector2 velocityDirection){}
    [Rpc(RpcMode.AnyPeer)] void Server_WeaponSwitch(int currentWeaponIndex){}
    [Rpc(RpcMode.AnyPeer)] void Server_Reload() {}

    [Rpc] void Client_UpdatePuppetPositions(Godot.Collections.Dictionary<long, Vector2> puppetPositions) {
        foreach (var kvp in puppetPositions) {
            GetNode<PuppetPlayer>("/root/world/" + kvp.Key).SetPuppetPosition(kvp.Value);
        }
    }
}