using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class SnapCard : Card, IBuffObtainable {
    public SnapCardStats stats { get ; private set; }
    public int PlayedOrder { get; private set; }

    [HideInInspector] public bool Revealed { get; private set; } = false;

    public UnityEvent CardRevealed = new UnityEvent();
    public UnityEvent CardPlayed = new UnityEvent();
    public UnityEvent BuffChanged = new UnityEvent();
    public List<Buff> buffs = new List<Buff>();
    public Location PlayedLocation { get; private set; }
    public Player ownedPlayer => PlayedLocation.player;

    public static float FlipCardDelay = 0.5f;

    private void OnEnable() {
        ActionSystem.SubscribeReaction<DrawCardGA>(OnCardDrawnReaction, ReactionTiming.POST);
    }
    public void OnCardDrawnReaction(DrawCardGA action) {
        //ActionSystem.Instance.AddReaction(new OnCardDrawnAction(this));
    }

    public void initCardStats(SnapCardStats stats) {
        this.stats = stats;
        SnapCurrencyCost currencyCost = GetComponent<SnapCurrencyCost>();
        if (currencyCost != null)
            currencyCost.SetBaseEnergyCost(stats.cost);
        
        Power power = GetComponent<Power>();
        if (power != null)
            power.SetBasePower(stats.power);
    }

    public void SetPlayedLocation(Location location) {
        PlayedLocation = location;
    }

    public IEnumerator revealCard() {
        SetFacing(CardFacing.FaceUp);
        Revealed = true;
        CardRevealed.Invoke();
        yield return new WaitForSeconds(FlipCardDelay);
        AbilityManager.Instance.ActivateOnRevealAbility(this);

        //List<Ability> abilities = new List<Ability>(GetComponents<Ability>());

        // abilities.ForEach(ability => {
        //     if(ability.definition.trigger == AbilityTrigger.OnReveal)
        //         RevealEventHandler.Instance.PushEvent(ability.ActivateAbility());
        // });
        yield return null;
    }

    public int GetPower() {
        return GetComponent<Power>().powerlevel;
    }

    public int GetBaseCost() {
        SnapCurrencyCost currencyCost = GetComponent<SnapCurrencyCost>();
        return currencyCost.GetBaseCost();
    }

    public int GetCurrentCost() {
        SnapCurrencyCost currencyCost = GetComponent<SnapCurrencyCost>();
        return currencyCost.GetCost();
    }

    public void ApplyBuff(Buff buff)
    {
        buffs.Add(buff);
        BuffChanged.Invoke();
    }
    
    public void RemoveBuff(Buff buff, bool replacingRemovedBuff = false)
    {
        buffs.Remove(buff);

        if(!replacingRemovedBuff)
            BuffChanged.Invoke();
    }

    public void RemoveAllBuffs()
    {
        buffs.Clear();
        BuffChanged.Invoke();
    }

    public bool IsBuffValid(Buff buff)
    {
        throw new System.NotImplementedException();
    }

    public bool HasKeyword(string keyword)
    {
        if (keyword=="OnReveal" || keyword=="Ongoing")
            return AbilityManager.Instance.GetAbilities()[this].Any(ability => ability.definition.trigger.ToString() == keyword);
        else{
            return buffs.Any(buff => buff.type.ToString() == keyword);
        }
    }
}