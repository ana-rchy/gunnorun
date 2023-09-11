using System;
using System.Collections.Generic;
using Godot;

public partial class Lobby : Node {
    LobbyManager LobbyManager;
    Button ReadyButton;

    public override void _Ready() {
        var slot1 = GetNode("Slot1");
        slot1.GetNode<Label>("Username").Text = Global.PlayerData.Username;
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) slot1.GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        RefreshList();

        LobbyManager = GetNode<LobbyManager>(Global.SERVER_PATH + "LobbyManager");
        ReadyButton = GetNode<Button>("Ready");
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public void RefreshList() {
        var players = new List<Global.PlayerDataStruct>(Global.OtherPlayerData.Values);

        for (int i = 2; i <= 8; i++) {
            GetNode<Panel>("Slot" + i.ToString()).Hide();
        }

        for (int i = 0; i < players.Count; i++) {
            var slot = GetNode<Panel>("Slot" + (i+2).ToString());

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

    void _OnReadyToggle(bool buttonPressed) {
        Global.PlayerData.ReadyStatus = !Global.PlayerData.ReadyStatus;

        var panel = (StyleBoxFlat) GetNode<Panel>("Slot1/ReadyIndicator").GetThemeStylebox("panel");
        var readyButton = GetNode<Button>("Ready");
        if (!buttonPressed) {
            panel.BgColor = new Color("cc0000");
            readyButton.Text = "Ready";
        } else {
            panel.BgColor = new Color("0ecc00");
            readyButton.Text = "Unready";
        }

        LobbyManager.Rpc(nameof(LobbyManager.Server_UpdateStatus), Global.PlayerData.ReadyStatus);
    }

    #endregion
}