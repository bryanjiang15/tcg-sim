public interface IBuffObtainable
{
    void ApplyBuff(Buff buff);
    void RemoveBuff(Buff buff, bool replacingRemovedBuff = false);
    void RemoveAllBuffs();
    bool IsBuffValid(Buff buff);

}
