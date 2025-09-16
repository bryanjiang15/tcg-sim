using System.Collections.Generic;
using UnityEngine;

public class UpdateStatGA : GameAction{
    public string statName;
    public int amount;

    public IEnumerable<IBuffObtainable> targets;
    public UpdateStatGA(string statName, int amount, IEnumerable<IBuffObtainable> targets) {
        this.statName = statName;
        this.amount = amount;
        this.targets = targets;
    }
}