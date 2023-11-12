using System;
using System.Threading.Tasks;
using Godot;

public partial class Player : RigidBody2D, IPlayer {
    [Export(PropertyHint.File)] public string TracerScene { get; private set; }
    [Export] public Timer ActionTimer { get; private set; }
    [Export] public Timer ReloadTimer { get; private set; }
    [Export] Timer RegenTimer;
    [Export] public RayCast2D WeaponRaycast { get; private set; }
    [Export] RayCast2D GroundRaycast;
    [Export] AnimatedSprite2D Sprite;
    [Export] Label Username;

    const float MAXIMUM_VELOCITY = 4000f;
    const float SPAWN_INTANGIBILITY_TIME = 2f;
    const int HP_REGEN = 5;
    const float DEATH_TIME = 3f;

    Weapon[] Weapons;
    public Weapon CurrentWeapon { get; private set; }
    public static int CurrentWeaponIndex { get; private set; } = 0; // needed to preserve weapon choice, but w/o weapon data
    float MomentumMultiplier;
    public int HP { get; private set; } = 100;

    public static Vector2 LastMousePos { get; private set; } = new Vector2(0, 0);
    public static (Vector2 StateVel, Vector2 VelSoftCap, Single ReelbackStrength) DebugData { get; private set; }
        = (new Vector2(0, 0), new Vector2(0, 0), 0f);

    public override void _Ready() {
        // signals
        if (Multiplayer.GetPeers().Length != 0) {
            var playerManager = this.GetNodeConst<PlayerManager>("PLAYER_MANAGER");
            WeaponShot += playerManager._OnWeaponShot;
            OtherPlayerHit += playerManager._OnOtherPlayerHit;
            HPChanged += playerManager._OnHPChanged;
            OnGround += playerManager._OnGround;
        }

        // etc
        Weapons = new Weapon[] { new Shotgun(), new Machinegun(), new RPG(), new Murasama() };
        CurrentWeapon = Weapons[CurrentWeaponIndex];
        EmitSignal(SignalName.WeaponChanged, CurrentWeapon.Name);
        Username.Text = Global.PlayerData.Username;
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) Sprite.Material)
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
            if (e.IsActionPressed($"Num{i}")) {
                CurrentWeapon = Weapons[i-1];
                CurrentWeaponIndex = i-1;

                EmitSignal(SignalName.WeaponChanged, CurrentWeapon.Name);
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
            EmitSignal(SignalName.OnGround, true, LinearVelocity.X);
        } else {
            EmitSignal(SignalName.OnGround, false, 0);
        }
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
    #region | funcs

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

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void WeaponShotEventHandler(Player player);
    [Signal] public delegate void WeaponReloadingEventHandler(string weaponName, float reloadTime, int baseAmmo);
    [Signal] public delegate void WeaponChangedEventHandler(string weaponName);
    [Signal] public delegate void OtherPlayerHitEventHandler(long playerID, int damage, string weaponName);
    [Signal] public delegate void HPChangedEventHandler(int newHP);
    [Signal] public delegate void OnGroundEventHandler(bool onGround, float xVel);
    [Signal] public delegate void OnDeathEventHandler(float deathTime);

    void _OnReplayOnly() {
        QueueFree();
    }

    #endregion
}
