public interface IBuffObtainable
{
    void ApplyBuff(Buff buff);
    void RemoveBuff(Buff buff);
    void RemoveAllBuffs();
    bool IsBuffValid(Buff buff);

}
