using Godot;
using System;

public partial class Crosshair : Node {
    [Export] Color _modulate = new Color(255, 255, 255);
    [Export] SubViewport _unfocusedCursor, _focusedCursor;
    Sprite2D _unfocusedSprite, _focusedSprite;

    public override void _Ready() {
        _unfocusedSprite = _unfocusedCursor.GetNode<Sprite2D>("Sprite");
        _focusedSprite = _focusedCursor.GetNode<Sprite2D>("Sprite");

        _unfocusedSprite.Modulate = _modulate;
        _focusedSprite.Modulate = _modulate;
    }

    public override void _Process(double _) {
        if (Input.IsActionPressed("Shoot")) {
            Input.SetCustomMouseCursor(_focusedCursor.GetTexture(), 0, _unfocusedSprite.Position);
        } else {
            Input.SetCustomMouseCursor(_unfocusedCursor.GetTexture(), 0, _focusedSprite.Position);
        }
    }

    public override void _ExitTree() {
        Input.SetCustomMouseCursor(null);
    }
}
