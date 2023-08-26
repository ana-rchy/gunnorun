using System;
using System.Linq;
using Godot;
using static Godot.GD;

public partial class Player : RigidBody2D {
    [Export] float MAXIMUM_VELOCITY = 4000f;

    PlayerUI UI;
    Timer ActionTimer;
    RayCast2D Raycast;

    public Weapon[] Weapons;
    public Weapon CurrentWeapon;
    float MomentumMultiplier;
    int HP = 100;

    public override void _Ready() {
        // set shader color
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) GetNode<Sprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        // node references
        UI = GetNode<PlayerUI>("PlayerUI");
        ActionTimer = GetNode<Timer>("ActionTimer");
        Raycast = GetNode<RayCast2D>("Raycast");

        // etc
        Weapons = new Weapon[] {new Shotgun(), new Machinegun(), new RPG()};
        CurrentWeapon = Weapons[0];
        GetNode<Label>("Username").Text = Global.PlayerData.Username;
        UI.SetAmmoText(CurrentWeapon.Ammo);
        UI.CurrentWeapon.Text = CurrentWeapon.Name;


        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer) {
            SetDeferred("name", Multiplayer.GetUniqueId().ToString());
        }

        // no-contact on spawn
        System.Threading.Thread t = new System.Threading.Thread(SpawnInvuln);
        t.Start();
    }

    //---------------------------------------------------------------------------------//
    #region | godot loops

    public override void _Input(InputEvent e) {
        for (int i = 1; i <= 3; i++) {
            if (e.IsActionPressed("Num" + i.ToString())) {
                CurrentWeapon = Weapons[i-1];

                UI.CurrentWeapon.Text = CurrentWeapon.Name;
                UI.SetAmmoText(CurrentWeapon.Ammo);
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        var ammoNotEmpty = CurrentWeapon.Ammo > 0 || CurrentWeapon.Ammo == null;
        
        if (Input.IsActionJustPressed("Reload") && CurrentWeapon.Ammo != CurrentWeapon.BaseAmmo) {
            Reload();

        } else if (Input.IsActionPressed("Shoot") && ActionTimer.IsStopped() && ammoNotEmpty && HP > 0) {
            Shoot();
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        if (LinearVelocity.DistanceTo(new Vector2(0, 0)) > MAXIMUM_VELOCITY) {
            var velocitySoftCap = LinearVelocity.Normalized() * MAXIMUM_VELOCITY;
            var reelbackStrength = ( 1 - 1/((state.LinearVelocity.DistanceTo(new Vector2(0, 0)) - MAXIMUM_VELOCITY) / 2) ) * CurrentWeapon.ReelbackStrength;
            state.LinearVelocity = state.LinearVelocity.MoveToward(velocitySoftCap, reelbackStrength);
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | main funcs

    public async void UpdateHP(int change) {
        HP += change;
        Label hpLabel = UI.GetNode<Label>("Control/HP");
        hpLabel.Text = HP.ToString();

        if (HP <= 0) {
            hpLabel.Text = "ur dead lol";
            await this.Sleep(3f);
            HP = 100;
            hpLabel.Text = "100";
        }
    }

    async void SpawnInvuln() {
        await this.Sleep(2f);
        SetCollisionMaskValue(2, true);
    }

    void Shoot() {
        var mousePosToPlayerPos = GetGlobalMousePosition().DirectionTo(GlobalPosition);
        // get the momentum-affected velocity, and add normal weapon knockback onto it
        LinearVelocity = (LinearVelocity * GetMomentumMultiplier(LinearVelocity, mousePosToPlayerPos)) + mousePosToPlayerPos.Normalized() * CurrentWeapon.Knockback;

        CurrentWeapon.Ammo--;
        UI.SetAmmoText(CurrentWeapon.Ammo);

        ActionTimer.Start(CurrentWeapon.Refire);

        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer) {
            Raycast.TargetPosition = -mousePosToPlayerPos.Normalized() * CurrentWeapon.Range;
            Raycast.ForceRaycastUpdate();

            if (Raycast.IsColliding()) {
                Node hitPlayer = (Node) Raycast.GetCollider();
                PlayerManager playerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
                
                playerManager.Rpc("Server_PlayerHit", long.Parse(hitPlayer.Name), CurrentWeapon.Damage);
            }
        }
    }

    void Reload() {
        CurrentWeapon.Ammo = CurrentWeapon.BaseAmmo;
        ActionTimer.Start(CurrentWeapon.Reload);
        UI.SetReloadText(CurrentWeapon);
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
