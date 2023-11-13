using System;
using System.Collections.Generic;
using Godot;

public partial class Lobby : Node {
    [Export] Button _readyButton;
    [Export] Panel _slot1;

    public override void _Ready() {
        Paths.AddNodePath("LOBBY", GetPath());

        if (Multiplayer.GetPeers().Length != 0) {
            ReadyToggled += this.GetNodeConst<LobbyManager>("LOBBY_MANAGER")._OnReadyToggled;
        }

        _slot1.GetNode<Label>("Username").Text = Global.PlayerData.Username;
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) _slot1.GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
        RefreshList();
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void RefreshList() {
        var players = new List<Global.PlayerDataStruct>(Global.OtherPlayerData.Values);

        for (int i = 2; i <= 8; i++) {
            GetNode<Panel>($"Slot{i}").Hide();
        }

        for (int i = 0; i < players.Count; i++) {
            var slot = GetNode<Panel>($"Slot{i+2}");

            slot.GetNode<Label>("Username").Text = players[i].Username;
            var playerColor = players[i].Color;
            ((ShaderMaterial) slot.GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

            var panel = (StyleBoxFlat) slot.GetNode<Panel>("ReadyIndicator").GetThemeStylebox("panel");
            if (!players[i].ReadyStatus) {
                panel.BgColor = new Color("cc0000");
            } else {
                panel.BgColor = new Color("0ecc00");
            }

            slot.Show();
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void ReadyToggledEventHandler(bool readyStatus);

    void _OnReadyToggle(bool buttonPressed) {
        Global.PlayerData.ReadyStatus = !Global.PlayerData.ReadyStatus;

        var panel = (StyleBoxFlat) _slot1.GetNode<Panel>("ReadyIndicator").GetThemeStylebox("panel");
        panel.BgColor = buttonPressed ? new Color("0ecc00") : new Color("cc0000");
        _readyButton.Text = !buttonPressed ? "Unready" : "Ready";

        EmitSignal(SignalName.ReadyToggled, Global.PlayerData.ReadyStatus);
    }

    #endregion
}