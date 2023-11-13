using Godot;
using System;

public partial class Tracer : Node2D {
    [Export] Node2D _area;

    [Export] float _speed;
    public float Range;

    public override void _PhysicsProcess(double delta) {
        _area.Position = new Vector2(_area.Position.X + _speed, _area.Position.Y);

        if (_area.Position.X >= Range) {
            QueueFree();
        }
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnCollision(Node2D body) {
        QueueFree();
    }

    #endregion
}