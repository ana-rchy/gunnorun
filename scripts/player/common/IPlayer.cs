using Godot;

public interface IPlayer {
    [Signal] public delegate void OnGroundEventHandler(float xVel);
    [Signal] public delegate void OffGroundEventHandler();
    
    public int GetHP();
    public virtual void ChangeHP(int newHP, bool emitSignal = true) {}
}