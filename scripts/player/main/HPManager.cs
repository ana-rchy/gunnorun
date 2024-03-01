using System;
using System.Threading.Tasks;
using Godot;

public partial class HPManager : Node {
    [Export] Player _player;
    [Export] Timer _regenTimer;

    const float SPAWN_INTANGIBILITY_TIME = 2f;
    const int HP_REGEN = 5;
    const float DEATH_TIME = 3f;

    public int HP { get; private set; } = 100;  

    //---------------------------------------------------------------------------------//
    #region | funcs

    // state-unpure
    public int GetHP() {
        return HP;
    }

    // side-effects
    public async Task Intangibility(float time) {
        _player.SetCollisionLayerValue(2, false);
        _player.SetCollisionMaskValue(4, false);
        await this.Sleep(time);
        _player.SetCollisionLayerValue(2, true);
        _player.SetCollisionMaskValue(4, true);
    }

    public async void ChangeHP(int newHP, bool callerIsClient = false) {
        if (HP <= 0) {
            return;
        }

        EmitSignal(SignalName.HPChanged, newHP);
        HP = newHP;

        if (HP <= 0) {
            EmitSignal(SignalName.Death, DEATH_TIME);
            await this.Sleep(DEATH_TIME);
            HP = 100;
            _ = Intangibility(SPAWN_INTANGIBILITY_TIME);
        }
    }

    void Regen() {
        if (_regenTimer.IsStopped() && HP > 0 && HP < 100) {
            ChangeHP(Math.Clamp(HP + HP_REGEN, 0, 100), true);
            _regenTimer.Start();
        }
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void HPChangedEventHandler(int newHP);
    [Signal] public delegate void DeathEventHandler(float deathTime);

    void _OnReplayOnly() {
        QueueFree();
    }

    #endregion
}
