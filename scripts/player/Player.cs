using System;
using System.Linq;
using Godot;
using static Godot.GD;

public partial class Player : RigidBody2D, IPlayer {
    [Export] float MAXIMUM_VELOCITY = 4000f;

    PlayerManager PlayerManager;
    PlayerUI UI;
    Timer ActionTimer;
    Timer ReloadTimer;
    RayCast2D Raycast;

    public Weapon[] Weapons;
    public Weapon CurrentWeapon;
    float MomentumMultiplier;
    int HP = 100;

    public static Vector2 LastMousePos = new Vector2(0, 0);

    public override void _Ready() {
        // set player color
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) GetNode<AnimatedSprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        // node references
        PlayerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
        UI = GetNode<PlayerUI>("PlayerUI");
        ActionTimer = GetNode<Timer>("ActionTimer");
        ReloadTimer = GetNode<Timer>("ReloadTimer");
        Raycast = GetNode<RayCast2D>("Raycast");

        // etc
        Weapons = new Weapon[] {new Shotgun(), new Machinegun(), new RPG()};
        CurrentWeapon = Weapons[0];
        GetNode<Label>("Username").Text = Global.PlayerData.Username;
        UI.SetAmmoText(CurrentWeapon.Ammo);
        UI.SelectedWeapon.Text = CurrentWeapon.Name;


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

                UI.SelectedWeapon.Text = CurrentWeapon.Name;
                UI.SetAmmoText(CurrentWeapon.Ammo);
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        var ammoNotEmpty = CurrentWeapon.Ammo > 0 || CurrentWeapon.Ammo == null;
        
        if (Input.IsActionJustPressed("Reload") && CurrentWeapon.Ammo != CurrentWeapon.BaseAmmo) {
            if (!ReloadTimer.IsStopped()) {
                UI.ReloadingWarning.Show();
            } else {
                UI.ReloadingWarning.Hide();
                Reload(CurrentWeapon);
            }

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
        if (HP <= 0) return;
        
        HP += change;
        UI.HP.Text = HP.ToString();

        if (HP <= 0) {
            UI.HP.Text = "ur dead lol";
            await this.Sleep(3f);
            HP = 100;
            UI.HP.Text = HP.ToString();
            SpawnInvuln();
        }
    }

    async void SpawnInvuln() {
        SetCollisionMaskValue(4, false);
        await this.Sleep(2f);
        SetCollisionMaskValue(4, true);
    }

    void Shoot() {
        LastMousePos = GetGlobalMousePosition();
        var mousePosToPlayerPos = LastMousePos.DirectionTo(GlobalPosition);
        LinearVelocity = (LinearVelocity * GetMomentumMultiplier(LinearVelocity, mousePosToPlayerPos)) + mousePosToPlayerPos.Normalized() * CurrentWeapon.Knockback;
        // ^ get the momentum-affected velocity, and add normal weapon knockback onto it

        CurrentWeapon.Ammo--;
        UI.SetAmmoText(CurrentWeapon.Ammo);
        ActionTimer.Start(CurrentWeapon.Refire);

        ShootTracer(-mousePosToPlayerPos);

        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer) {
            CheckPlayerHit(-mousePosToPlayerPos);
        }
    }

    async void Reload(Weapon reloadingWeapon) {
        reloadingWeapon.Ammo = 0; // prevent firing remaining ammo while reloading
        UI.SetReloadText(reloadingWeapon);
        ReloadTimer.Start(reloadingWeapon.Reload); // prevent reloading in quick succession, and reloading 2+ weapons
        await this.Sleep(reloadingWeapon.Reload); // prevent having ammo to fire while should be reloading
        reloadingWeapon.Ammo = reloadingWeapon.BaseAmmo;
        UI.ReloadingWarning.Hide();
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | organization funcs

    float GetMomentumMultiplier(Vector2 currentVelocity, Vector2 mousePosToPlayerPos) {
        float angleDelta = currentVelocity.AngleTo(mousePosToPlayerPos);
        if (Mathf.RadToDeg(angleDelta) <= 45) // if less than 45 degrees change, keep all momentum
            return 1f;
        
        angleDelta -= MathF.Round(MathF.PI / 4, 4);

        return Math.Clamp(MathF.Round((MathF.Cos(4/3 * angleDelta) + 1) / 2, 4), 0, 1); // scale the momentum over a range of 135*
    }

    void ShootTracer(Vector2 playerPosToMousePos) {
        var tracerScene = GD.Load<PackedScene>("res://scenes/player/Tracer.tscn");
        var tracer = tracerScene.Instantiate<Tracer>();

        tracer.GlobalPosition = GlobalPosition;
        tracer.Rotation = (new Vector2(0, 0)).AngleToPoint(playerPosToMousePos);
        tracer.Range = CurrentWeapon.Range;

        AddSibling(tracer);

        if (Multiplayer.MultiplayerPeer is not OfflineMultiplayerPeer) {
            PlayerManager.Rpc(nameof(PlayerManager.Server_TracerShot), tracer.Rotation, tracer.Range);
        }
    }

    void CheckPlayerHit(Vector2 playerPosToMousePos) {
        Raycast.TargetPosition = playerPosToMousePos.Normalized() * CurrentWeapon.Range;
        Raycast.ForceRaycastUpdate();

        if (Raycast.IsColliding()) {
            Node hitPlayer = (Node) Raycast.GetCollider();
            
            PlayerManager.Rpc(nameof(PlayerManager.Server_PlayerHit), long.Parse(hitPlayer.Name), CurrentWeapon.Damage);
        }
    }

    #endregion
}
