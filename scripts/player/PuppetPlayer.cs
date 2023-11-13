using System;
using System.Threading.Tasks;
using Godot;

public partial class PuppetPlayer : CharacterBody2D, IPlayer {
    [Export(PropertyHint.File)] string TracerScene;
    [Export] ColorRect GreenHP;

    public Vector2 PuppetPosition { get; set; }
    public int HP { get; private set; } = 100;

    double Timer;

    public override void _PhysicsProcess(double delta) {
        if (Timer >= Global.TICK_RATE) {
            var tween = CreateTween();
            tween.TweenProperty(this, "global_position", PuppetPosition, Global.TICK_RATE);
            Timer -= Global.TICK_RATE;
        }
        
        Timer += delta;
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    public int GetHP() {
        return HP;
    }

    public async void ChangeHP(int newHP, bool emitSignal = true) {
        if (HP <= 0) {
            return;
        }

        HP = newHP;
        GreenHP.Size = new Vector2((HP / 100f) * 200f, 20f);

        if (HP <= 0) {
            await this.Sleep(3f);
            HP = 100;
            GreenHP.Size = new Vector2(200f, 20f);
            SpawnInvuln();
        }
    }

    public void SpawnTracer(float rotation, float range) {
        var tracer = GD.Load<PackedScene>(TracerScene).Instantiate<Tracer>();

        tracer.GlobalPosition = GlobalPosition;
        tracer.Rotation = rotation;
        tracer.Range = range;

        var tracerArea = tracer.GetNode<Area2D>("Area2D");
        tracerArea.SetCollisionMaskValue(4, false);
        tracerArea.SetCollisionMaskValue(2, true);

        this.GetNodeConst("WORLD").AddChild(tracer);
    }

    async void SpawnInvuln() {
        SetCollisionLayerValue(2, false);
        await this.Sleep(2f);
        SetCollisionLayerValue(2, true);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | funcs

    [Signal] public delegate void OnGroundEventHandler(bool onGround, float xVel = 0);

    #endregion
}