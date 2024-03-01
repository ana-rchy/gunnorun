using System;
using Godot;
using GC = Godot.Collections;

public partial class DebugRecorder : Node {
    public static GC.Dictionary<string, Variant> LastDebugData { get; private set; }
    
    [Export] Player _player;

    GC.Array<Vector2> _positionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> _mousePositionsList = new GC.Array<Vector2>();
    GC.Array<Vector2> _velocityList = new GC.Array<Vector2>();
    GC.Array<string> _weaponList = new GC.Array<string>();

    GC.Array<Vector2> _stateVelocityList = new GC.Array<Vector2>();
    GC.Array<Vector2> _velocityCapList = new GC.Array<Vector2>();
    GC.Array<Single> _reelbackStrengthList = new GC.Array<Single>();

    public override void _Ready() {
        Paths.AddNodePath("DEBUG_RECORDER", GetPath());
    }

    public override void _PhysicsProcess(double delta) {
        _positionsList.Add(_player.Position);
        //_mousePositionsList.Add(Player.LastMousePos);
        _velocityList.Add(_player.LinearVelocity);
        //_weaponList.Add(_player.CurrentWeapon.Name);

        //_stateVelocityList.Add(Player.DebugData.StateVel);
        //_velocityCapList.Add(Player.DebugData.VelSoftCap);
        //_reelbackStrengthList.Add(Player.DebugData.ReelbackStrength);
    }
    
    //---------------------------------------------------------------------------------//
	#region | signals

    public void _OnRaceFinished(float finishTime, string playerName) {
        LastDebugData = new GC.Dictionary<string, Variant>() {
            { "Positions", _positionsList },
            { "MousePositions", _mousePositionsList },
            { "Velocities", _velocityList },
            { "Weapons", _weaponList },

            { "StateVelocities", _stateVelocityList },
            { "VelocityCaps", _velocityCapList },
            { "ReelbackStrengths", _reelbackStrengthList }
        };
    }

    #endregion
}
