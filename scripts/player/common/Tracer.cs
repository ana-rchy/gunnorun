using Godot;
using System;

public partial class Tracer : Node2D {
    [Export] Node2D Area;

    [Export] float Speed;
    public float Range;

    public override void _PhysicsProcess(double delta) {
        Area.Position = new Vector2(Area.Position.X + Speed, Area.Position.Y);

        if (Area.Position.X >= Range) {
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