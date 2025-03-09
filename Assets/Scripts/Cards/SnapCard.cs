using System.Collections;
using System.Collections.Generic;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

public struct SnapCardStats {
    public int power;
    public int cost;
    public string card_name;
    public int series;

    public SnapCardStats(int power, int cost, string name, int series) {
        this.power = power;
        this.cost = cost;
        this.card_name = name;
        this.series = series;
    }
}

public class SnapCard : Card {
    public SnapCardStats stats { get ; private set; }

    [HideInInspector] public bool revealed { get; private set; } = false;

    public UnityEvent CardRevealed = new UnityEvent();
    public UnityEvent CardPlayed = new UnityEvent();

    public List<Buff> buffs = new List<Buff>();

    public void initCardStats(SnapCardStats stats) {
        this.stats = stats;
        SnapCurrencyCost currencyCost = GetComponent<SnapCurrencyCost>();
        currencyCost.SetAmount("Gems", stats.cost);
        
        Power power = GetComponent<Power>();
        power.SetPower(stats.power);
    }

    public void AddBuff(Buff buff) {
        buffs.Add(buff);
        switch(buff.type) {
            case BuffType.AdditionalPower:
                StatBuff statBuff = (StatBuff)buff;
                GetComponent<Power>().AddPower(statBuff.amount);
                break;
        }
    }

    public void SetupAbility(AbilityDefinition abilityDefinition) {
        var ability = gameObject.AddComponent<Ability>();
        ability.definition = abilityDefinition;
    }

    public IEnumerator revealCard() {
        SetFacing(CardFacing.FaceUp);
        revealed = true;
        CardRevealed.Invoke();

        List<Ability> abilities = new List<Ability>(GetComponents<Ability>());

        // abilities.ForEach(ability => {
        //     if(ability.definition.trigger == AbilityTrigger.OnReveal)
        //         RevealEventHandler.Instance.PushEvent(ability.ActivateAbility());
        // });
        yield return null;
    }
}