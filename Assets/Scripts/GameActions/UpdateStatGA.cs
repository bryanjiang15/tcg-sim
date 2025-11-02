using System.Collections.Generic;
using UnityEngine;

public class UpdateStatGA : GameAction{
    public SnapCard source;
    public StatTypeModal statType;
    public int amount;

    public IEnumerable<IBuffObtainable> targets;
    public UpdateStatGA(StatTypeModal statType, int amount, IEnumerable<IBuffObtainable> targets, SnapCard source) {
        this.statType = statType;
        this.amount = amount;
        this.targets = targets;
    }
}