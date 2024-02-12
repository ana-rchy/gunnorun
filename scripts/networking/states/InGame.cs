using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;
using MsgPack.Serialization;

public partial class InGame : State {
    public override void _Ready() {
        Paths.AddNodePath("IN_GAME_STATE", GetPath());
    }

    //---------------------------------------------------------------------------------//
    #region | rpc

    [Rpc(RpcMode.AnyPeer, TransferMode = TransferModeEnum.UnreliableOrdered)] void Server_UpdatePlayerPosition(Vector2 position) {}
    [Rpc(RpcMode.AnyPeer)] void Server_WeaponShot(string name, float rotation, float range) {}
    [Rpc(RpcMode.AnyPeer)] void Server_Intangibility(long id, float time) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerHPChanged(long id, int newHP) {}
    [Rpc(RpcMode.AnyPeer)] void Server_PlayerFrameChanged(byte frame) {}
    
    [Rpc] void Client_PlayerLeft(long id) {
		Global.OtherPlayerData.Remove(id);

        GetNode($"{Paths.GetNodePath("WORLD")}/{id}").QueueFree();
    }

    [Rpc(TransferMode = TransferModeEnum.UnreliableOrdered)] void Client_UpdatePuppetPositions(byte[] puppetPositionsSerialized) {
        if (!IsActiveState()) return;
        
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
        if (id == Multiplayer.GetUniqueId() || !IsActiveState()) {
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

    [Rpc] void Client_Intangibility(long id, float time) {
        if (!IsActiveState()) return;
        
        var player = GetNode<IPlayer>($"{Paths.GetNodePath("WORLD")}/{id}");
        Task.Run(() => {
            _ = player.Intangibility(time);
        });
    }

    [Rpc] void Client_PlayerHPChanged(long id, int newHP) {
        if (!IsActiveState()) return;

        var player = GetNode<IPlayer>($"{Paths.GetNodePath("WORLD")}/{id}");
        player.ChangeHP(newHP);
    }


    [Rpc] void Client_PlayerFrameChanged(long id, int frame) {
        if (id == Multiplayer.GetUniqueId() || !IsActiveState()) {
            return;
        }

        var playerSprite = GetNode<AnimatedSprite2D>($"{Paths.GetNodePath("WORLD")}/{id}/Sprite");

        playerSprite.Frame = frame;
    }

    [Rpc] void Client_LapChanged(int lap, int maxLaps) {
        if (!IsActiveState()) return;
        
        var lapManager = this.GetNodeConst<Lap>("LAP");
        lapManager.EmitSignal(Lap.SignalName.LapPassed, lap, maxLaps);
    }



    [Rpc] void Client_PlayerWon(long id, double time) {
        if (!IsActiveState()) return;

        var name = Multiplayer.GetUniqueId() == id ? Global.PlayerData.Username : Global.OtherPlayerData[id].Username;
        
        var playerUI = GetNode<PlayerUI>($"{Paths.GetNodePath("WORLD")}/{Multiplayer.GetUniqueId()}/PlayerUI");
        playerUI._OnRaceFinished((float) Math.Round(time, 3), name);
    }

    [Rpc] void Client_LoadWorld(string worldName) {
        if (!IsActiveState()) return;

        Global.PlayerData.ReadyStatus = false;
        worldName = worldName.Replace(".remap", "");

        StateMachine.ChangeState("LoadingWorld", new() {{ "world", worldName }} );
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    public void _OnWeaponShot(Player player) {
        if (!IsActiveState()) return;

        var playerPosToMousePos = player.GlobalPosition.DirectionTo(player.GetGlobalMousePosition());
        Rpc(nameof(Server_WeaponShot), player.CurrentWeapon.Name,
            new Vector2(0, 0).AngleToPoint(playerPosToMousePos), player.CurrentWeapon.Range);
    }

    public void _OnOtherPlayerHit(long playerID, int newHP, string weaponName) {
        if (!IsActiveState()) return;

        Rpc(nameof(Server_PlayerHPChanged), playerID, newHP);

        if (weaponName == "Murasama") {
            Rpc(nameof(Server_Intangibility), playerID, Murasama.INTANGIBILITY_TIME);
        }
    }

    public void _OnHPChanged(int newHP) {
        if (!IsActiveState()) return;
        
        Rpc(nameof(Server_PlayerHPChanged), Multiplayer.GetUniqueId(), newHP);
    }

    public void _OnPlayerFrameChanged(int frame) {
        if (!IsActiveState()) return;

        Rpc(nameof(Server_PlayerFrameChanged), frame);
    }

    #endregion
}
