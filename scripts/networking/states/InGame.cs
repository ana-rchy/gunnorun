using System;
using System.Collections.Generic;
using Godot;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;
using MsgPack.Serialization;

public partial class InGame : State {
	[Rpc(RpcMode.AnyPeer, TransferMode = TransferModeEnum.UnreliableOrdered)] void Server_UpdatePlayerPosition(Vector2 position) {}

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
}