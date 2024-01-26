using System;
using Godot;

public partial class GunRotation : AnimatedSprite2D {
    Node2D _parent;

    public override void _Ready() {
        _parent = GetParent<Node2D>();

        if (Multiplayer.GetPeers().Length != 0) {
            //PlayerFrameChanged += this.GetNodeConst<PlayerManager>("PLAYER_MANAGER")._OnPlayerFrameChanged;
        }
    }


    public override void _Input(InputEvent e) {
        if (e is not InputEventMouseMotion) {
            return;
        }

        var prevFrame = Frame;

        var normal = new Vector2(0, -1);
        var angle = normal.AngleTo(_parent.GetLocalMousePosition());

        Frame = angle >= 0 ? 0 : 5;
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
        
        if (Frame != prevFrame) {
            EmitSignal(SignalName.PlayerFrameChanged, Frame);
        }
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void PlayerFrameChangedEventHandler(int frame);

    #endregion
}