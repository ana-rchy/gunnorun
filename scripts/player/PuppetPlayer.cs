using Godot;
using System;

public partial class PuppetPlayer : RigidBody2D, IPlayer {
	public void SetPuppetPosition(Vector2 puppetPosition) {
		var tween = CreateTween();
		tween.TweenProperty(this, "global_position", puppetPosition, Global.TICK_RATE);
	}
}
