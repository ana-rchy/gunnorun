using System;
using System.Threading.Tasks;
using Godot;

public partial class PuppetPlayer : CharacterBody2D, IPlayer {
    [Export(PropertyHint.File)] string _tracerScene;
    [Export] RayCast2D _groundRaycast;
    [Export] ColorRect _greenHP;

    const float SPAWN_INTANGIBILITY_TIME = 2f;
    const float DEATH_TIME = 3f;

    public Vector2 PuppetPosition { get; set; }
    public int HP { get; private set; } = 100;

    double _timer;
    Vector2 _lastPosition = new Vector2(0, 0);

    public override void _PhysicsProcess(double delta) {
        if (_timer >= Global.TICK_RATE) {
            var tween = CreateTween();
            tween.TweenProperty(this, "global_position", PuppetPosition, Global.TICK_RATE);

            if (_groundRaycast.IsColliding()) {
                EmitSignal(SignalName.OnGround, true, (GlobalPosition.X - _lastPosition.X) / Global.TICK_RATE);
            } else {
                EmitSignal(SignalName.OnGround, false, 0);
            }

            _lastPosition = GlobalPosition;
            _timer -= Global.TICK_RATE;
        }

        _timer += delta;
    }

    //---------------------------------------------------------------------------------//
    #region | funcs
    
    // ???
    public int GetHP() {
        return HP;
    }

    // side-effects
    public async Task Intangibility(float time) {
        SetCollisionLayerValue(4, false);
        SetCollisionMaskValue(2, false);
        await this.Sleep(time);
        SetCollisionLayerValue(4, true);
        SetCollisionMaskValue(2, true);
    }

    public async void ChangeHP(int newHP, bool callerIsClient = false) {
        if (HP <= 0) {
            return;
        }

        HP = newHP;
        _greenHP.Size = new Vector2((HP / 100f) * 200f, 20f);

        if (HP <= 0) {
            await this.Sleep(DEATH_TIME);
            HP = 100;
            _greenHP.Size = new Vector2(200f, 20f);
            _ = Intangibility(SPAWN_INTANGIBILITY_TIME);
        }
    }

    public void SpawnTracer(float rotation, float range) {
        var tracer = GD.Load<PackedScene>(_tracerScene).Instantiate<Tracer>();

        tracer.GlobalPosition = GlobalPosition;
        tracer.Rotation = rotation;
        tracer.Range = range;

        var tracerArea = tracer.GetNode<Area2D>("Area2D");
        tracerArea.SetCollisionMaskValue(4, false);
        tracerArea.SetCollisionMaskValue(2, true);

        this.GetNodeConst("WORLD").AddChild(tracer);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void OnGroundEventHandler(bool onGround, float xVel);

    #endregion
}
