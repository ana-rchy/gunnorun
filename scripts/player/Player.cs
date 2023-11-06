using System;
using System.Threading.Tasks;
using Godot;

public partial class Player : RigidBody2D, IPlayer {
    public Timer ActionTimer { get; private set; }
    public Timer ReloadTimer { get; private set; }
    Timer RegenTimer;
    public RayCast2D WeaponRaycast { get; private set; }
    RayCast2D GroundRaycast;

    const float MAXIMUM_VELOCITY = 4000f;
    const float SPAWN_INTANGIBILITY_TIME = 2f;
    const int HP_REGEN = 5;
    const float DEATH_TIME = 3f;

    Weapon[] Weapons;
    public Weapon CurrentWeapon { get; private set; }
    public static int CurrentWeaponIndex { get; private set; } = 0; // needed to perserve weapon choice, but w/o weapon data
    float MomentumMultiplier;
    public int HP { get; private set; } = 100;

    public static Vector2 LastMousePos { get; private set; } = new Vector2(0, 0);
    public static (Vector2 StateVel, Vector2 VelSoftCap, Single ReelbackStrength) DebugData { get; private set; }
        = (new Vector2(0, 0), new Vector2(0, 0), 0f);
    // public static Vector2 Debug_VelocitySoftCap { get; private set; } = new Vector2(0, 0);
    // public static Single Debug_ReelbackStrength { get; private set; } = 0f;
    // public static Vector2 Debug_StateVelocity { get; private set; } = new Vector2(0, 0);

    public override void _Ready() {
        // node references
        ActionTimer = GetNode<Timer>("Timers/ActionTimer");
        ReloadTimer = GetNode<Timer>("Timers/ReloadTimer");
        RegenTimer = GetNode<Timer>("Timers/RegenTimer");
        WeaponRaycast = GetNode<RayCast2D>("Raycasts/WeaponRaycast");
        GroundRaycast = GetNode<RayCast2D>("Raycasts/GroundRaycast");

        // signals
        if (Multiplayer.GetPeers().Length != 0) {
            var playerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
            OtherPlayerHit += playerManager._OnOtherPlayerHit;
            HPChanged += playerManager._OnHPChanged;
        }

        // etc
        Weapons = new Weapon[] { new Shotgun(), new Machinegun(), new RPG(), new Murasama() };
        CurrentWeapon = Weapons[CurrentWeaponIndex];
        EmitSignal(SignalName.WeaponChanged, CurrentWeapon.Name);
        GetNode<Label>("Username").Text = Global.PlayerData.Username;
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) GetNode<AnimatedSprite2D>("Sprite").Material)
            .SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        if (Multiplayer.GetPeers().Length != 0) {
            SetDeferred("name", Multiplayer.GetUniqueId().ToString());
            Task.Run(() => Intangibility(SPAWN_INTANGIBILITY_TIME));
        }
    }

    //---------------------------------------------------------------------------------//
    #region | godot loops

    public override void _Input(InputEvent e) {
        for (int i = 1; i <= 4; i++) {
            if (e.IsActionPressed("Num" + i.ToString())) {
                CurrentWeapon = Weapons[i-1];
                CurrentWeaponIndex = i-1;

                EmitSignal(SignalName.WeaponChanged, CurrentWeapon.Name);
                //UI.ChangeWeapon(CurrentWeapon.Name);
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (Input.IsActionJustPressed("Reload") && ReloadTimer.IsStopped()) {
            CurrentWeapon.ReloadWeapon(this);
        } else if (Input.IsActionPressed("Shoot") && ActionTimer.IsStopped() && HP > 0) {
            LastMousePos = GetGlobalMousePosition();
            CurrentWeapon.Shoot(this);
        }

        Regen();
        if (GroundRaycast.IsColliding()) {
            EmitSignal(SignalName.OnGround, LinearVelocity.X);
        } else {
            EmitSignal(SignalName.OffGround);
        }
        //ParticlesManager.EmitGrinding(LinearVelocity.X);
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        if (LinearVelocity.DistanceTo(new Vector2(0, 0)) > MAXIMUM_VELOCITY) {
            var velocitySoftCap = LinearVelocity.Normalized() * MAXIMUM_VELOCITY;
            var reelbackStrength = 1 - ( 1 / (0.0000001f * state.LinearVelocity.DistanceTo(velocitySoftCap) + 1) ); // just plug this shit into desmos man
            state.LinearVelocity = state.LinearVelocity.MoveToward(velocitySoftCap, state.LinearVelocity.DistanceTo(velocitySoftCap) * reelbackStrength);

            DebugData = (DebugData.StateVel, velocitySoftCap, reelbackStrength);
        }

        DebugData = (state.LinearVelocity, DebugData.VelSoftCap, DebugData.ReelbackStrength);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | main funcs

    public async Task Intangibility(float time) {
        SetCollisionMaskValue(4, false);
        await this.Sleep(time);
        SetCollisionMaskValue(4, true);
    }

    public int GetHP() {
        return HP;
    }

    public async void ChangeHP(int newHP, bool emitSignal = true) {
        if (HP <= 0) {
            return;
        }

        if (emitSignal) {
            EmitSignal(SignalName.HPChanged, newHP);
        }
        HP = newHP;

        if (HP <= 0) {
            EmitSignal(SignalName.OnDeath, DEATH_TIME);
            await this.Sleep(DEATH_TIME);
            HP = 100;
            _ = Intangibility(SPAWN_INTANGIBILITY_TIME);
        }
    }

    void Regen() {
        if (RegenTimer.IsStopped() && HP > 0 && HP < 100) {
            ChangeHP(HP + HP_REGEN);
            RegenTimer.Start();
        }
    }

    // async void Reload(Weapon reloadingWeapon) {
    //     reloadingWeapon.Ammo = 0; // prevent firing remaining ammo while reloading

    //     UI.Reload(reloadingWeapon.Name, reloadingWeapon.Reload);
    //     ReloadTimer.Start(reloadingWeapon.Reload); // prevent reloading in quick succession, and reloading 2+ weapons
    //     await this.Sleep(reloadingWeapon.Reload); // prevent having ammo to fire while should be reloading
        
    //     reloadingWeapon.Ammo = reloadingWeapon.BaseAmmo;
    //     UI.UpdateAmmo(reloadingWeapon.Name, reloadingWeapon.Ammo);
    // }

    // public void ShootTracer(Vector2 playerPosToMousePos) {
    //     var tracerScene = GD.Load<PackedScene>("res://scenes/player/Tracer.tscn");
    //     var tracer = tracerScene.Instantiate<Tracer>();

    //     tracer.GlobalPosition = GlobalPosition;
    //     tracer.Rotation = (new Vector2(0, 0)).AngleToPoint(playerPosToMousePos);
    //     tracer.Range = CurrentWeapon.Range;

    //     AddSibling(tracer);

    //     // if (Multiplayer.GetPeers().Length != 0) {
    //     //     PlayerManager.Rpc(nameof(PlayerManager.Server_TracerShot), tracer.Rotation, tracer.Range);
    //     // }
    // }

    // public void CheckPlayerHit(Vector2 playerPosToMousePos) {
    //     WeaponRaycast.TargetPosition = playerPosToMousePos.Normalized() * CurrentWeapon.Range;
    //     WeaponRaycast.ForceRaycastUpdate();

    //     if (WeaponRaycast.IsColliding()) {
    //         Node hitPlayer = (Node) WeaponRaycast.GetCollider();
            
    //         PlayerManager.Rpc(nameof(PlayerManager.Server_PlayerHit), long.Parse(hitPlayer.Name), CurrentWeapon.Damage);

    //         if (CurrentWeapon is Murasama) {
    //             PlayerManager.Rpc(nameof(PlayerManager.Server_MurasamaIntangibility), long.Parse(hitPlayer.Name));
    //         }
    //     }
    // }

    // public async Task UpdateHP(int change) {
    //     if (HP <= 0) return;
        
    //     HP += change;
    //     UI.HP.SetDeferred("text", HP.ToString());

    //     if (HP <= 0) {
    //         UI.HP.SetDeferred("text", "ur dead lol");
    //         await this.Sleep(3f);
    //         HP = 100;
    //         UI.HP.Text = HP.ToString();
    //         _ = SpawnInvuln();
    //     }
    // }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void WeaponShotEventHandler(Player player);
    [Signal] public delegate void WeaponReloadingEventHandler(string weaponName, float reloadTime, int baseAmmo);
    [Signal] public delegate void WeaponChangedEventHandler(string weaponName);
    [Signal] public delegate void OtherPlayerHitEventHandler(long playerID, int damage, string weaponName);
    [Signal] public delegate void HPChangedEventHandler(int newHP);
    [Signal] public delegate void OnDeathEventHandler(float deathTime);
    [Signal] public delegate void OnGroundEventHandler(float xVel);
    [Signal] public delegate void OffGroundEventHandler();

    void _OnReplayOnly() {
        QueueFree();
    }

    #endregion
}
