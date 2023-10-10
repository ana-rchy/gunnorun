using System;
using System.Threading.Tasks;
using System.Linq;
using Godot;
using static Godot.GD;

public partial class Player : RigidBody2D, IPlayer {
    [Export] float MAXIMUM_VELOCITY = 4000f;

    PlayerManager PlayerManager;
    public PlayerUI UI;
    public Timer ActionTimer;
    Timer ReloadTimer;
    RayCast2D Raycast;

    public Weapon[] Weapons;
    public Weapon CurrentWeapon;
    public static int CurrentWeaponIndex = 0; // needed to perserve weapon choice, but not weapon data
    float MomentumMultiplier;
    int HP = 100;

    public static Vector2 LastMousePos = new Vector2(0, 0);
    public static Vector2 Debug_VelocitySoftCap = new Vector2(0, 0);
    public static Single Debug_ReelbackStrength = 0f;
    public static Vector2 Debug_StateVelocity = new Vector2(0, 0);

    public override void _Ready() {
        // set player color
        var playerColor = Global.PlayerData.Color;
        ((ShaderMaterial) GetNode<AnimatedSprite2D>("Sprite").Material).SetShaderParameter("color", new Vector3(playerColor.R, playerColor.G, playerColor.B));

        // node references
        PlayerManager = GetNode<PlayerManager>(Global.SERVER_PATH + "PlayerManager");
        UI = GetNode<PlayerUI>("PlayerUI");
        ActionTimer = GetNode<Timer>("Timers/ActionTimer");
        ReloadTimer = GetNode<Timer>("Timers/ReloadTimer");
        Raycast = GetNode<RayCast2D>("Raycast");

        // etc
        Weapons = new Weapon[] { new Shotgun(), new Machinegun(), new RPG(), new Murasama() };
        CurrentWeapon = Weapons[CurrentWeaponIndex];
        UI.ChangeWeapon(CurrentWeapon.Name);
        GetNode<Label>("Username").Text = Global.PlayerData.Username;


        if (Multiplayer.GetPeers().Length != 0) {
            SetDeferred("name", Multiplayer.GetUniqueId().ToString());
        }

        // no-contact on spawn, healing
        Task.Run(SpawnInvuln);
        Task.Run(Regen);

    }

    //---------------------------------------------------------------------------------//
    #region | godot loops

    public override void _Input(InputEvent e) {
        for (int i = 1; i <= 4; i++) {
            if (e.IsActionPressed("Num" + i.ToString())) {
                CurrentWeapon = Weapons[i-1];
                CurrentWeaponIndex = i-1;

                UI.ChangeWeapon(CurrentWeapon.Name);
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        var ammoNotEmpty = CurrentWeapon.Ammo > 0 || CurrentWeapon.Ammo == null;
        
        if (Input.IsActionJustPressed("Reload") && CurrentWeapon.Ammo != CurrentWeapon.BaseAmmo && ReloadTimer.IsStopped()) {
            Reload(CurrentWeapon);

        } else if (Input.IsActionPressed("Shoot") && ActionTimer.IsStopped() && ammoNotEmpty && HP > 0) {
            CurrentWeapon.Shoot(this);
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        if (LinearVelocity.DistanceTo(new Vector2(0, 0)) > MAXIMUM_VELOCITY) {
            var velocitySoftCap = LinearVelocity.Normalized() * MAXIMUM_VELOCITY;
            var reelbackStrength = 1 - ( 1 / (0.0000001f * state.LinearVelocity.DistanceTo(velocitySoftCap) + 1) ); // just plug this shit into desmos man
            // var reelbackStrength = ( 1 - 1/( (state.LinearVelocity.DistanceTo(velocitySoftCap) + 100) / 100) ) * CurrentWeapon.ReelbackStrength;
            state.LinearVelocity = state.LinearVelocity.MoveToward(velocitySoftCap, state.LinearVelocity.DistanceTo(velocitySoftCap) * reelbackStrength);

            Debug_VelocitySoftCap = velocitySoftCap;
            Debug_ReelbackStrength = reelbackStrength;
        }

        Debug_StateVelocity = state.LinearVelocity;
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | main funcs

    public async void UpdateHP(int change) {
        if (HP <= 0) return;
        
        HP += change;
        UI.HP.SetDeferred("text", HP.ToString());

        if (HP <= 0) {
            UI.HP.SetDeferred("text", "ur dead lol");
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

    async void Reload(Weapon reloadingWeapon) {
        reloadingWeapon.Ammo = 0; // prevent firing remaining ammo while reloading

        ReloadTimer.Start(reloadingWeapon.Reload); // prevent reloading in quick succession, and reloading 2+ weapons
        UI.Reload(reloadingWeapon.Name, ReloadTimer, reloadingWeapon.Reload);
        await this.Sleep(reloadingWeapon.Reload); // prevent having ammo to fire while should be reloading
        
        reloadingWeapon.Ammo = reloadingWeapon.BaseAmmo;
        UI.UpdateAmmo(reloadingWeapon.Name, reloadingWeapon.Ammo);
    }

    async void Regen() {
        while (true) {
            if (HP > 0 && HP < 100) {
                await this.Sleep(1.5f);
                UpdateHP(5);
            }
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | organization funcs

    public void ShootTracer(Vector2 playerPosToMousePos) {
        var tracerScene = GD.Load<PackedScene>("res://scenes/player/Tracer.tscn");
        var tracer = tracerScene.Instantiate<Tracer>();

        tracer.GlobalPosition = GlobalPosition;
        tracer.Rotation = (new Vector2(0, 0)).AngleToPoint(playerPosToMousePos);
        tracer.Range = CurrentWeapon.Range;

        AddSibling(tracer);

        if (Multiplayer.GetPeers().Length != 0) {
            PlayerManager.Rpc(nameof(PlayerManager.Server_TracerShot), tracer.Rotation, tracer.Range);
        }
    }

    public void CheckPlayerHit(Vector2 playerPosToMousePos) {
        Raycast.TargetPosition = playerPosToMousePos.Normalized() * CurrentWeapon.Range;
        Raycast.ForceRaycastUpdate();

        if (Raycast.IsColliding()) {
            Node hitPlayer = (Node) Raycast.GetCollider();
            
            PlayerManager.Rpc(nameof(PlayerManager.Server_PlayerHit), long.Parse(hitPlayer.Name), CurrentWeapon.Damage);

            if (CurrentWeapon is Murasama) {
                PlayerManager.Rpc(nameof(PlayerManager.Server_MurasamaIntangibility), long.Parse(hitPlayer.Name));
            }
        }
    }

    #endregion
}
