using System;
using System.Linq;
using Godot;

public partial class MultiplayerPanel : MainPanel {
    public override void _Ready() {
        base._Ready();

        JoinPressed += GetNode<Client>(Global.SERVER_PATH)._OnJoinPressed;
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void JoinPressedEventHandler(string ip, int port);

    void _OnJoinPressed() {
        Global.PlayerData.Username = UsernameField.Text;
        Global.PlayerData.Color = ColorField.Color;

        var ip = GetNode<LineEdit>("IP").Text;
        ip = ip == "" ? "localhost" : ip; // localhost by default, entered ip otherwise
        var port = (int) GetNode<SpinBox>("Port").Value;

        EmitSignal(SignalName.JoinPressed, ip, port);
    }

    void _OnUsernameChanged(string text) {
        Global.PlayerData.Username = text;

        GetNode<LineEdit>("/root/Menu/TabContainer/Singleplayer/Panel/Username").Text = text;
    }

    void _OnColorChanged(Color color) {
        Global.PlayerData.Color = color;

        GetNode<ColorPickerButton>("/root/Menu/TabContainer/Singleplayer/Panel/PlayerColor").Color = color;
    }

    #endregion
}