using Godot;
using System;

public partial class Player : RigidBody2D {
    [Export] public Timer ActionTimer { get; private set; }
    [Export] public Timer ReloadTimer { get; private set; }
    [Export] public RayCast2D WeaponRaycast { get; private set; }
    
    [Export] AnimatedSprite2D _sprite;
    [Export] Label _username;
    [Export] RayCast2D _groundRaycast;

    public override void _Ready() {
        Paths.AddNodePath("PLAYER", GetPath());
    }
}
