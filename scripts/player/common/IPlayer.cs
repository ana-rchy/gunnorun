public interface IPlayer {
    int GetHP();
    void ChangeHP(int newHP, bool emitSignal = true);
}