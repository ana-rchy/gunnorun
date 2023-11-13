using System;
using Godot;

public partial class MultiplayerPanel : MainPanel {
    [Export] LineEdit _IP;
    [Export] SpinBox _port;

    public override void _Ready() {
        base._Ready();

        JoinPressed += this.GetNodeConst<Client>("SERVER")._OnJoinPressed;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void JoinPressedEventHandler(string ip, int port);

    void _OnJoinPressed() {
        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;

        var ip = _IP.Text;
        ip = (ip == "") ? "localhost" : ip; // localhost by default, entered ip otherwise
        var port = (int) _port.Value;

        EmitSignal(SignalName.JoinPressed, ip, port);
    }

    #endregion
}