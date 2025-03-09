
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;

public enum BuffType
{
    AdditionalPower,
    AdditionalCost,
    AdditionEnergy,
    AdditionalMaxEnergy,
    SetPower,
    SetCost,
    Indestructible,
    Unmovable,
    Unplayable,
    Silenced,
    Created,
}
public class Buff
{
    public BuffType type;
}

public class StatBuff : Buff
{
    public int amount;

    public StatBuff(BuffType type, int amount=0)
    {
        this.type = type;
        this.amount = amount;
    }
}

public class UnplayableBuff : Buff
{
    public List<LocationPosition> locations;

    public UnplayableBuff(List<LocationPosition> locations)
    {
        type = BuffType.Unplayable;
        this.locations = locations;
    }
}
