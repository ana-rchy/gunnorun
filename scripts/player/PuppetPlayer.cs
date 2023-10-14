using System;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

public partial class PuppetPlayer : CharacterBody2D, IPlayer {
    double Timer;
    public Vector2 PuppetPosition;
    int HP = 100;

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

    public async Task UpdateHP(int change) {
        if (HP <= 0) return;

        var greenHP = GetNode<ColorRect>("GreenHP");

        HP += change;
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

    #endregion
}