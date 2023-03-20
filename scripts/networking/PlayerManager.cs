using Godot;
using System;
using static Godot.MultiplayerApi;

public partial class PlayerManager : Node {
    [Rpc(RpcMode.AnyPeer)] void RPC_HandleShoot(Vector2 velocityDirection){}
    [Rpc(RpcMode.AnyPeer)] void RPC_HandleWeaponSwitch(int currentWeaponIndex){}
    [Rpc(RpcMode.AnyPeer)] void RPC_HandleReload() {}
}