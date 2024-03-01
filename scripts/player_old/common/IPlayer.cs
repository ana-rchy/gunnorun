using System.Threading.Tasks;
using Godot;

public interface IPlayer {
    [Signal] public delegate void OnGroundEventHandler(float xVel);
    [Signal] public delegate void OffGroundEventHandler();

    public async Task Intangibility(float time) {}
    public int GetHP();
    public void ChangeHP(int newHP, bool callerIsClient = false) {}
}
