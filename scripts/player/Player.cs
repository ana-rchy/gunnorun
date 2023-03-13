using Godot;
using static Godot.GD;
using System;

public partial class Player : RigidBody2D {
    public override void _Ready() {
        var playerColor = Global.PlayerColor;
        ((ShaderMaterial) GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));
    }

    public override void _Process(double delta) {
        if (Input.IsActionPressed("Shoot")) {
            SetKnockback();
        }

        LinearVelocity.Clamp(new Vector2(-2000f, -2000f), new Vector2(2000f, 2000f));
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    void SetKnockback() {
        var mouseToPlayerPos = GetGlobalMousePosition().DirectionTo(GlobalPosition);
        var knockback = 2000f;

        LinearVelocity = mouseToPlayerPos.Normalized() * knockback;

        GD.Print("aasdsadhf");
    }

    #endregion
}
