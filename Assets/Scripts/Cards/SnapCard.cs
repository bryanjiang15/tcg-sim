using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

public struct SnapCardStats {
    private Dictionary<string, CardStat> stats;
    public string card_name;
    public int card_id;
    public int series;

    public SnapCardStats(int power, int cost, string name, int series, int id) {
        this.card_name = name;
        this.card_id = id;
        this.series = series;
        this.stats = new Dictionary<string, CardStat>();
        
        // Initialize with default stats
        stats["power"] = new CardStat("power", power);
        stats["cost"] = new CardStat("cost", cost);
    }

    // Indexer to access stats by name
    public int this[string statName] {
        get {
            if (stats.ContainsKey(statName)) {
                return stats[statName].statValue;
            }
            return 0; // Default value if stat doesn't exist
        }
        set {
            if (stats.ContainsKey(statName)) {
                stats[statName] = new CardStat(statName, value);
            } else {
                stats[statName] = new CardStat(statName, value);
            }
        }
    }

    // Method to add a new stat
    public void AddStat(string statName, int statValue) {
        stats[statName] = new CardStat(statName, statValue);
    }

    // Method to get a CardStat object
    public CardStat GetCardStat(string statName) {
        if (stats.ContainsKey(statName)) {
            return stats[statName];
        }
        return null;
    }

    // Method to check if a stat exists
    public bool HasStat(string statName) {
        return stats.ContainsKey(statName);
    }

    // Method to get all stat names
    public IEnumerable<string> GetStatNames() {
        return stats.Keys;
    }
}

public class SnapCard : Card, IBuffObtainable, IDestructible, IDiscardable, IMoveable { 
    public SnapCardStats stats { get ; private set; }
    public int PlayedOrder { get; private set; }

    [SerializeField] private GameObject highlight;
    public bool IsSelectable { get; private set; } = false;
    public bool IsSelected { get; private set; } = false;

    [HideInInspector] public bool Revealed { get; private set; } = false;

    public UnityEvent CardRevealed = new UnityEvent();
    public UnityEvent CardPlayed = new UnityEvent();
    public UnityEvent BuffChanged = new UnityEvent();
    public List<Buff> buffs = new List<Buff>();
    public Location PlayedLocation { get; private set; }
    public List<Ability> abilities = new List<Ability>();
    public SnapContext context;
    public virtual Player ownedPlayer => GroupRegistry.Instance.GetOwnerIndex(Group) == 0 ? Player.Player1 : Player.Player2;

    Vector3 originalScale; // Original scale of the card for resizing during drag
    private void Start() {
        BoxCollider2D collider = GetComponentInChildren<BoxCollider2D>();
        if (collider != null) {
            originalScale = collider.size; // Store the original size of the collider for resizing during drag
        }
        context = new SnapContext();
    }

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
            currencyCost.SetBaseEnergyCost(stats["cost"]);
        
        Power power = GetComponent<Power>();
        if (power != null)
            power.SetBasePower(stats["power"]);
    }

    public void SetPlayedLocation(Location location) {
        PlayedLocation = location;
    }

    public IEnumerator revealCard(bool IsCardPlayed = true) {
        if (IsCardPlayed){
            SetFacing(CardFacing.FaceUp);
        }
        Revealed = true;
        CardRevealed.Invoke();
        if (IsCardPlayed)
            yield return new WaitForSeconds(FlipCardDelay);
        // AbilityManager.Instance.ActivateOnRevealAbility(this);

        //List<Ability> abilities = new List<Ability>(GetComponents<Ability>());

        // abilities.ForEach(ability => {
        //     if(ability.definition.trigger == AbilityTrigger.OnReveal)
        //         RevealEventHandler.Instance.PushEvent(ability.ActivateAbility());
        // });
        yield return null;
    }

    public virtual int GetPower() {
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
        if (buff.type == BuffType.SetPower){
            //Remove other SetPower buffs
            buffs.RemoveAll(b => b.type == BuffType.SetPower || b.type == BuffType.AdditionalPower);
        }
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
            return AbilityManager.Instance.GetAbilities()[this].Any(ability => ability.definition.triggerDefinition.triggerType.ToString() == keyword);
        else{
            return buffs.Any(buff => buff.type.ToString() == keyword);
        }
    }

    public void MinimizeColliderOnDrag(bool isDragging) {
        // Resize collider when dragging to be smaller
        BoxCollider2D boxCollider = GetComponentInChildren<BoxCollider2D>(); // Get the BoxCollider component attached to the card
        if (boxCollider == null) {
            return;
        }
        if (isDragging) {
            // Reduce the size of the collider when dragging
            boxCollider.size = new Vector3(originalScale.x * 0.5f, originalScale.y * 0.5f, originalScale.z);
        } else {
            // Reset the size of the collider to its original size when not dragging
            boxCollider.size = originalScale; // Use the stored original scale
        }
    } 

    public void SetSelectable(bool isSelectable) {
        IsSelectable = isSelectable;
        highlight.SetActive(isSelectable);
    }

    public void SelectCard() {
        if (IsSelectable) {
            if (IsSelected) {
                IsSelected = false;
                highlight.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0.1f, 0.75f);
            } else {
                IsSelected = true;
                highlight.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    /*  Public Actions */

    public void GainPower(int power, SnapCard source) {
        StatBuff powerBuff = new StatBuff(BuffType.AdditionalPower, source, power);
        ApplyBuff(powerBuff);
    }

    public void GainCost(int cost, SnapCard source) {
        StatBuff costBuff = new StatBuff(BuffType.AdditionalCost, source, cost);
        ApplyBuff(costBuff);
    }

    public void SetPower(int power, SnapCard source) {
        StatBuff powerBuff = new StatBuff(BuffType.SetPower, source, power);
        ApplyBuff(powerBuff);
    }

    public void SetCost(int cost, SnapCard source) {
        StatBuff costBuff = new StatBuff(BuffType.SetCost, source, cost);
        ApplyBuff(costBuff);
    }

    public void DestroyCard(SnapCard source) {
        if (PlayedLocation != null) {
            GetComponent<DestroyCardOperator>().Activate();
        }
    }
    
    public bool canBeDestroyed() {
        return PlayedLocation != null;
    }

    public void DiscardCard(SnapCard source)
    {
        //if (PlayedLocation == null) { // If the card is in hand, discard it
        GetComponent<DiscardCardOperator>().Activate();
        //}
    }

    public bool canBeDiscarded() {
        return PlayedLocation == null;
    }

    public void MoveCard(SnapCard source, LocationPosition locationPosition)
    {
        if (PlayedLocation != null)
        {
            PlayedLocation.cardGroup.UnMount(this);
            Location newLocation = TargetSystem.Instance.GetLocation(locationPosition, ownedPlayer);
            newLocation.cardGroup.Mount(this);
            PlayedLocation = newLocation;
        }
    }

    public bool canBeMoved() {
        return PlayedLocation != null;
    }

    public virtual AbilityAmount GetTargetValue(AbilityRequirementType reqType, AbilityAmount reqAmount = null)
    {
        switch (reqType)
        {
            case AbilityRequirementType.Power:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = GetPower().ToString() };
            case AbilityRequirementType.Cost:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = GetBaseCost().ToString() };
            case AbilityRequirementType.CurrentCost:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = GetCurrentCost().ToString() };
            case AbilityRequirementType.HasKeyword:
                return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = "false" };
            case AbilityRequirementType.IsCreated:
                return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = "false" };
            case AbilityRequirementType.CardName:
                return new AbilityAmount { amountType = AbilityAmountType.Cardid, value = stats.card_name };
            case AbilityRequirementType.BuffPresent:
                return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = "false" };
            case AbilityRequirementType.LocationFull:
                if (PlayedLocation != null)
                    return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = PlayedLocation.isFull().ToString() };
                return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = "false" };
            case AbilityRequirementType.HasTag:
                if (reqAmount == null)
                    return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = "false" };
                bool hasTag = stats.GetCardStat(reqAmount.value) != null;
                return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = hasTag.ToString() };
            default:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = "0" };
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Debug.Log($"Card Name: {stats.card_name}, Power: {stats["power"]}, Cost: {stats["cost"]}, Series: {stats.series}");
        if (buffs != null && buffs.Count > 0) {
            for(int i = 0; i < buffs.Count; i++) {
                Buff buff = buffs[i];
                StatBuff statBuff = buff as StatBuff;
                Debug.Log($"- Buff Type: {statBuff.type}, Amount: {statBuff.amount}, Source: {statBuff.source.stats.card_name}, count: {i}");
            }
        } else {
            Debug.Log($"No buffs on {stats.card_name}");
        }
    }
#endif
}