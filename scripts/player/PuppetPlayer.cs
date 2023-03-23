using Godot;
using System;

public partial class PuppetPlayer : RigidBody2D {
    const float TICK_RATE = 1 / 60;

    Tween Tween;

    public override void _Ready() {
        Tween = CreateTween();
    }


    public void SetPuppetPosition(Vector2 puppetPosition) {
        Tween.TweenProperty(this, "global_position", puppetPosition, TICK_RATE);
    }
}