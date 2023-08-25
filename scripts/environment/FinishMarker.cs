using Godot;
using System;

public partial class FinishMarker : Node {
    private void _OnPlayerEntered(Node2D body) {
        GD.Print(body);
        GetTree().ChangeSceneToFile("res://scenes/UI/Menu.tscn");
    }
}