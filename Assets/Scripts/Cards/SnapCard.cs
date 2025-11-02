using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

public struct SnapCardStats {
    private Dictionary<StatTypeModal, CardStat> statsByType;
    private Dictionary<string, StatTypeModal> nameToType;
    public string card_name;
    public int card_id;
    public int series;

    public SnapCardStats(string name, int series, int id) {
        this.card_name = name;
        this.card_id = id;
        this.series = series;
        this.statsByType = new Dictionary<StatTypeModal, CardStat>();
        this.nameToType = new Dictionary<string, StatTypeModal>();
    }

    // Backward-compatible constructor
    public SnapCardStats(int power, int cost, string name, int series, int id) {
        this.card_name = name;
        this.card_id = id;
        this.series = series;
        this.statsByType = new Dictionary<StatTypeModal, CardStat>();
        this.nameToType = new Dictionary<string, StatTypeModal>();

        var powerType = new StatTypeModal { Name = "Power", StatValueType = StatValueType.ValueStat };
        var costType = new StatTypeModal { Name = "Cost", StatValueType = StatValueType.ValueStat };
        AddStat(powerType, power);
        AddStat(costType, cost);
    }

    // Indexer by StatTypeModal
    public int this[StatTypeModal statType] {
        get {
            if (statType != null && statsByType.ContainsKey(statType)) {
                return statsByType[statType].statValue;
            }
            return 0;
        }
        set {
            if (statType != null) {
                AddStat(statType, value);
            }
        }
    }

    // Indexer by name (compatibility)
    public int this[string statName] {
        get {
            if (string.IsNullOrEmpty(statName)) return 0;
            if (nameToType.TryGetValue(statName, out var type) && statsByType.TryGetValue(type, out var cardStat)) {
                return cardStat.statValue;
            }
            return 0;
        }
        set {
            if (string.IsNullOrEmpty(statName)) return;
            if (!nameToType.TryGetValue(statName, out var type)) {
                type = new StatTypeModal { Name = statName, StatValueType = StatValueType.ValueStat };
            }
            AddStat(type, value);
        }
    }

    // Add or update stat by type
    public void AddStat(StatTypeModal statType, int statValue) {
        if (statType == null) return;
        statsByType[statType] = new CardStat(statType, statValue);
        if (!string.IsNullOrEmpty(statType.Name)) {
            nameToType[statType.Name] = statType;
        }
    }

    // Convenience overload by name
    public void AddStat(string statName, int statValue) {
        if (string.IsNullOrEmpty(statName)) return;
        var type = nameToType.ContainsKey(statName) ? nameToType[statName] : new StatTypeModal { Name = statName, StatValueType = StatValueType.ValueStat };
        AddStat(type, statValue);
    }

    // Get a CardStat by type
    public CardStat GetCardStat(StatTypeModal statType) {
        if (statType != null && statsByType.ContainsKey(statType)) {
            return statsByType[statType];
        }
        return null;
    }

    // Get a CardStat by name (compatibility)
    public CardStat GetCardStat(string statName) {
        if (!string.IsNullOrEmpty(statName) && nameToType.TryGetValue(statName, out var type)) {
            return GetCardStat(type);
        }
        return null;
    }

    public bool HasStat(StatTypeModal statType) {
        return statType != null && statsByType.ContainsKey(statType);
    }

    public bool HasStat(string statName) {
        return !string.IsNullOrEmpty(statName) && nameToType.ContainsKey(statName);
    }

    public IEnumerable<string> GetStatNames() {
        return nameToType.Keys;
    }
}

public class SnapCard : Card, IBuffObtainable, IDestructible, IDiscardable, IMoveable { 
    public SnapCardStats stats { get ; private set; }
    public int PlayedOrder { get; private set; }
    public int cardInstanceId { get; private set; }

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

    public void SetCardInstanceId(int instanceId) {
        this.cardInstanceId = instanceId;
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
        // If it's a StatBuff, apply the stat change
        if (buff is StatBuff statBuff && statBuff.statType != null)
        {
            ApplyStatBuff(statBuff);
        }
        
        buffs.Add(buff);
        BuffChanged.Invoke();
    }

    private void ApplyStatBuff(StatBuff statBuff)
    {
        if (statBuff.statType == null) return;
        
        int currentValue = stats[statBuff.statType];
        int newValue = currentValue;
        
        switch (statBuff.buffModifierType)
        {
            case BuffModifierType.Add:
                newValue = currentValue + statBuff.amount;
                break;
            case BuffModifierType.Subtract:
                newValue = currentValue - statBuff.amount;
                break;
            case BuffModifierType.Multiply:
                newValue = currentValue * statBuff.amount;
                break;
            case BuffModifierType.Set:
                newValue = statBuff.amount;
                break;
        }
        
        // Update the stats struct properly
        var updatedStats = stats;
        updatedStats[statBuff.statType] = newValue;
        stats = updatedStats;
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
            return buffs.Any(buff => buff.statType?.Name == keyword);
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
        var powerStatType = new StatTypeModal { Name = "Power", StatValueType = StatValueType.ValueStat };
        StatBuff powerBuff = new StatBuff(powerStatType, BuffModifierType.Add, power, source?.cardInstanceId ?? 0);
        ApplyBuff(powerBuff);
    }

    public void GainCost(int cost, SnapCard source) {
        var costStatType = new StatTypeModal { Name = "Cost", StatValueType = StatValueType.ValueStat };
        StatBuff costBuff = new StatBuff(costStatType, BuffModifierType.Add, cost, source?.cardInstanceId ?? 0);
        ApplyBuff(costBuff);
    }

    public void SetPower(int power, SnapCard source) {
        var powerStatType = new StatTypeModal { Name = "Power", StatValueType = StatValueType.ValueStat };
        StatBuff powerBuff = new StatBuff(powerStatType, BuffModifierType.Set, power, source?.cardInstanceId ?? 0);
        ApplyBuff(powerBuff);
    }

    public void SetCost(int cost, SnapCard source) {
        var costStatType = new StatTypeModal { Name = "Cost", StatValueType = StatValueType.ValueStat };
        StatBuff costBuff = new StatBuff(costStatType, BuffModifierType.Set, cost, source?.cardInstanceId ?? 0);
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
                Debug.Log($"- Buff StatType: {statBuff.statType?.Name}, Amount: {statBuff.amount}, Modifier: {statBuff.buffModifierType}, count: {i}");
            }
        } else {
            Debug.Log($"No buffs on {stats.card_name}");
        }
    }
#endif
}