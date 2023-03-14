using Godot;
using static Godot.GD;
using System;

public partial class Player : RigidBody2D {
    [Export] float MAXIMUM_VELOCITY = 4000f;

    WeaponManager WeaponManager;

    Vector2 Velocity;
    float MomentumMultiplier;

    public override void _Ready() {
        // set shader color
        var playerColor = Global.PlayerColor;
        ((ShaderMaterial) GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        WeaponManager = GetNode<WeaponManager>("WeaponManager");
    }

    //---------------------------------------------------------------------------------//
    #region | physics loop

    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        state.LinearVelocity = ClampVelocity();
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | main funcs

    public Vector2 GetVelocity(float knockback) {
        var mousePosToPlayerPos = GetGlobalMousePosition().DirectionTo(GlobalPosition);

        // get the momentum-affected velocity, and add normal weapon knockback onto it
        return (LinearVelocity * GetMomentumMultiplier(LinearVelocity, mousePosToPlayerPos)) + mousePosToPlayerPos.Normalized() * knockback;
    }

    Vector2 ClampVelocity() {
        if (LinearVelocity.DistanceTo(new Vector2(0, 0)) > MAXIMUM_VELOCITY) {
            return LinearVelocity.Normalized() * MAXIMUM_VELOCITY;
        }
        return LinearVelocity;
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | utility funcs

    float GetMomentumMultiplier(Vector2 currentVelocity, Vector2 mousePosToPlayerPos) {
        float angleDelta = currentVelocity.AngleTo(mousePosToPlayerPos);
        if (Mathf.RadToDeg(angleDelta) <= 45) // if less than 45 degrees change, keep all momentum
            return 1f;
        
        angleDelta -= MathF.PI / 4;

        return (MathF.Cos((4/3) * angleDelta) + 1) / 2; // scale the momentum over a range of 135*
    }

    #endregion
}
