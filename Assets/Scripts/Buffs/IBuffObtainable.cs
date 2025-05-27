public interface IBuffObtainable : ITargetable
{
    void ApplyBuff(Buff buff);
    void RemoveBuff(Buff buff, bool replacingRemovedBuff = false);
    void RemoveAllBuffs();
    bool IsBuffValid(Buff buff);
    void GainPower(int power, SnapCard source);
    void GainCost(int cost, SnapCard source);
    void SetPower(int power, SnapCard source);
    void SetCost(int cost, SnapCard source);
}
