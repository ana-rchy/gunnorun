using Godot;

public interface IPlayer {
    [Signal] public delegate void OnGroundEventHandler(float xVel);
    [Signal] public delegate void OffGroundEventHandler();
    
    public int GetHP();
    public void ChangeHP(int newHP, bool callerIsClient = false) {}
}