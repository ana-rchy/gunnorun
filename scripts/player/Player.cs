using Godot;
using static Godot.GD;
using System;
using System.Linq;

public partial class Player : RigidBody2D {
    [Export] float MAXIMUM_VELOCITY = 4000f;

    PlayerManager PlayerManager;
    PlayerUI UI;
    Timer ActionTimer;

    public Weapon[] Weapons;
    public Weapon CurrentWeapon;
    float MomentumMultiplier;

    public override void _Ready() {
        // set shader color
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        // node references
        PlayerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
        UI = GetNode<PlayerUI>("PlayerUI");
        ActionTimer = GetNode<Timer>("ActionTimer");

        // etc
        Weapons = new Weapon[] {new Shotgun(), new Machinegun(), new RPG()};
        CurrentWeapon = Weapons[0];
        GetNode<Label>("Username").Text = Global.PlayerData.Username;
        UI.SetAmmoText(CurrentWeapon.Ammo);
        UI.CurrentWeapon.Text = CurrentWeapon.Name;

        Tween = CreateTween();

        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer)
            Name = Multiplayer.GetUniqueId().ToString();
    }

    //---------------------------------------------------------------------------------//
    #region | godot loops

    public override void _Input(InputEvent e) {
        for (int i = 1; i <= 3; i++) {
            if (e.IsActionPressed("Num" + i.ToString())) {
                CurrentWeapon = Weapons[i-1];

                UI.CurrentWeapon.Text = CurrentWeapon.Name;
                UI.SetAmmoText(CurrentWeapon.Ammo);
                
                if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer)
                    PlayerManager.Rpc("Server_WeaponSwitch", Array.IndexOf(Weapons, CurrentWeapon));
            }
        }
    }

    double TickTimer;
    public override void _PhysicsProcess(double delta) {
        var ammoNotEmpty = CurrentWeapon.Ammo > 0 || CurrentWeapon.Ammo == null;
        
        if (Input.IsActionJustPressed("Reload") && CurrentWeapon.Ammo != CurrentWeapon.BaseAmmo) {
            CurrentWeapon.Ammo = CurrentWeapon.BaseAmmo;
            ActionTimer.Start(CurrentWeapon.Reload);
            UI.SetReloadText(CurrentWeapon);

        } else if (Input.IsActionPressed("Shoot") && ActionTimer.IsStopped() && ammoNotEmpty) {
            SetVelocity();

            CurrentWeapon.Ammo--;
            UI.SetAmmoText(CurrentWeapon.Ammo);

            ActionTimer.Start(CurrentWeapon.Refire);
        }

        if (TickTimer >= Global.TICK_RATE) {
            PlayerManager.Rpc("Server_UpdatePlayerPosition", GlobalPosition);
            TickTimer -= Global.TICK_RATE;
        }
        TickTimer += delta;
    }

    Tween Tween;
    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        if (LinearVelocity.DistanceTo(new Vector2(0, 0)) > MAXIMUM_VELOCITY) {
            var velocitySoftCap = LinearVelocity.Normalized() * MAXIMUM_VELOCITY;
            var reelbackStrength = ( 1 - 1/((state.LinearVelocity.DistanceTo(new Vector2(0, 0)) - MAXIMUM_VELOCITY) / 2) ) * CurrentWeapon.ReelbackStrength;
            state.LinearVelocity = state.LinearVelocity.MoveToward(velocitySoftCap, reelbackStrength);
            Print(state.LinearVelocity);
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | main funcs

    void SetVelocity() {
        var mousePosToPlayerPos = GetGlobalMousePosition().DirectionTo(GlobalPosition);

        // get the momentum-affected velocity, and add normal weapon knockback onto it
        LinearVelocity = (LinearVelocity * GetMomentumMultiplier(LinearVelocity, mousePosToPlayerPos)) + mousePosToPlayerPos.Normalized() * CurrentWeapon.Knockback;
    }

    // deprecated
    void ClampVelocity(ref Vector2 velocity) {
        if (LinearVelocity.DistanceTo(new Vector2(0, 0)) > MAXIMUM_VELOCITY) {
            velocity = LinearVelocity.Normalized() * MAXIMUM_VELOCITY;
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | utility funcs

    float GetMomentumMultiplier(Vector2 currentVelocity, Vector2 mousePosToPlayerPos) {
        float angleDelta = currentVelocity.AngleTo(mousePosToPlayerPos);
        if (Mathf.RadToDeg(angleDelta) <= 45) // if less than 45 degrees change, keep all momentum
            return 1f;
        
        angleDelta -= MathF.Round(MathF.PI / 4, 4);

        return MathF.Round((MathF.Cos(4/3 * angleDelta) + 1) / 2, 4); // scale the momentum over a range of 135*
    }

    #endregion
}
