using System;
using Godot;
using static Godot.MultiplayerApi;

public partial class GunRotation : AnimatedSprite2D {
    Node2D Parent;
    PlayerManager PlayerManager;

    public override void _Ready() {
        Parent = GetParent<Node2D>();
        PlayerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
    }


    public override void _Process(double delta) {
        var normal = new Vector2(0, -1);
        var angle = normal.AngleTo(Parent.GetLocalMousePosition());


        if (angle >= 0) {
            Frame = 0;
        } else {
            Frame = 5;
        }

        var absAngle = MathF.Abs(angle);
        var pi8th = MathF.PI / 8; // 22.5*
        if (absAngle >= 0 && absAngle < pi8th) {
            Frame += 0;
        } else if (absAngle > pi8th && absAngle < pi8th * 3) {
            Frame += 1;
        } else if (absAngle > pi8th * 3 && absAngle < pi8th * 5) {
            Frame += 2;
        } else if (absAngle > pi8th * 5 && absAngle < pi8th * 7) {
            Frame += 3;
        } else if (absAngle > pi8th * 7) {
            Frame += 4;
        }

        PlayerManager.Rpc(nameof(PlayerManager.Server_PlayerFrameChanged), Frame);
    }
}