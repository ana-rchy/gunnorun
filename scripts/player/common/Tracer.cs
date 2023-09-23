using Godot;
using System;

public partial class Tracer : Node2D {
    Node2D Area;

    [Export] float Speed;
    public float Range;

    public override void _Ready() {
        Area = GetNode<Node2D>("Area2D");
    }

    public override void _PhysicsProcess(double delta) {
        Area.Position = new Vector2(Area.Position.X + Speed, Area.Position.Y);

        if (Area.Position.X >= Range) QueueFree();
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    private void _OnCollision(Node2D body) {
        QueueFree();
    }

    #endregion
}