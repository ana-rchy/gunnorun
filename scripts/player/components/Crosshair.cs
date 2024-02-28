using Godot;
using System;

public partial class Crosshair : Node {
    [Export] Color _modulateColor = new Color(255, 255, 255);
    [Export] SubViewport _unfocusedCursor, _focusedCursor;

    public override void _Ready() {
        _unfocusedCursor.GetNode<Sprite2D>("Sprite").Modulate = _modulateColor;
        _focusedCursor.GetNode<Sprite2D>("Sprite").Modulate = _modulateColor;
    }

    public override void _Process(double _) {
        if (Input.IsActionPressed("Shoot")) {
            Input.SetCustomMouseCursor(_focusedCursor.GetTexture());
        } else {
            Input.SetCustomMouseCursor(_unfocusedCursor.GetTexture());
        }
    }

    public override void _ExitTree() {
        Input.SetCustomMouseCursor(null);
    }
}
