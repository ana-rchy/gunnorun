using System;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

public partial class PuppetPlayer : CharacterBody2D, IPlayer {
    double Timer;
    public Vector2 PuppetPosition;
    public int HP { get; private set; } = 100;

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

    public async void ChangeHP(int newHP) {
        if (HP <= 0) return;

        var greenHP = GetNode<ColorRect>("GreenHP");

        HP = newHP;
        greenHP.Size = new Vector2((HP / 100f) * 200f, 20f);

        if (HP <= 0) {
            await this.Sleep(3f);
            HP = 100;
            greenHP.Size = new Vector2(200f, 20f);
            SpawnInvuln();
        }
    }

    async void SpawnInvuln() {
        SetCollisionLayerValue(2, false);
        await this.Sleep(2f);
        SetCollisionLayerValue(2, true);
    }

    public void SpawnTracer(float rotation, float range) {
        var tracerScene = Load<PackedScene>("res://scenes/player/Tracer.tscn");
        var tracer = tracerScene.Instantiate<Tracer>();

        tracer.GlobalPosition = GlobalPosition;
        tracer.Rotation = rotation;
        tracer.Range = range;

        var tracerArea = tracer.GetNode<Area2D>("Area2D");
        tracerArea.SetCollisionMaskValue(4, false);
        tracerArea.SetCollisionMaskValue(2, true);

        GetNode(Global.WORLD_PATH).AddChild(tracer);
    }

    #endregion
}