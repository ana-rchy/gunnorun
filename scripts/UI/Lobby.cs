using Godot;
using System;

public partial class Lobby : Panel {
    const string WORLD_PATH = "res://scenes/worlds/";

    SceneTree Tree;

    public override void _Ready() {
        Tree = GetTree();
    } 

    //---------------------------------------------------------------------------------//
    #region | signals

    private void _OnSingleplayerPressed() {
        Tree.ChangeSceneToFile(WORLD_PATH + "Cave.tscn");
        Global.PlayerColor = GetNode<ColorPickerButton>("PlayerColor").Color;
    }

    #endregion
}
