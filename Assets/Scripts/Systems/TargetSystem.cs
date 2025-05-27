using System.Collections.Generic;
using System.Linq;
using CardHouse;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Events;

public class TargetSystem : MonoBehaviour {
    public static TargetSystem Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public List<ITargetable> GetTargets(List<AbilityTargetDefinition> targetDefinitions, SnapCard owner, GameAction triggeredAction = null) {
        List<ITargetable> targets = new List<ITargetable>();
        foreach (AbilityTargetDefinition targetDefinition in targetDefinitions) {
            targets.AddRange(GetTargets(targetDefinition, owner, triggeredAction));
        }
        return targets;
    }

    public List<ITargetable> GetTargets(AbilityTargetDefinition targetDefinition, SnapCard owner, GameAction triggeredAction = null) {
        List<ITargetable> targets = new List<ITargetable>();
        Location ownerLocation = owner.PlayedLocation;
        Player enemyPlayer = owner.ownedPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        switch (targetDefinition.target)
        {
            case AbilityTarget.Self:
                targets.Add(owner);
                break;
            case AbilityTarget.Deck:
                targets = GroupRegistry.Instance.Get(GroupName.Deck, (int)owner.ownedPlayer).MountedCards.OfType<ITargetable>().ToList();
                break;
            case AbilityTarget.Hand:
                targets = GroupRegistry.Instance.Get(GroupName.Hand, (int)owner.ownedPlayer).MountedCards.OfType<ITargetable>().ToList();
                break;
            case AbilityTarget.EnemyDeck:
                targets = GroupRegistry.Instance.Get(GroupName.Deck, (int)enemyPlayer).MountedCards.OfType<ITargetable>().ToList();
                break;
            case AbilityTarget.EnemyHand:
                targets = GroupRegistry.Instance.Get(GroupName.Hand, (int)enemyPlayer).MountedCards.OfType<ITargetable>().ToList();
                break;
            case AbilityTarget.PlayerDirectLocationCards:
                targets = ownerLocation.cardGroup.MountedCards.OfType<ITargetable>().ToList();
                break;
            case AbilityTarget.EnemyDirectLocationCards:
                Location enemyLocation = GetEnemyLocation(ownerLocation);
                targets = enemyLocation.cardGroup.MountedCards.OfType<ITargetable>().ToList();
                break;
            case AbilityTarget.AllPlayerCards:
                targets = FindObjectsByType<SnapCard>(FindObjectsSortMode.None).Where(card =>
                    card.ownedPlayer == owner.ownedPlayer && !(card is LocationCard)).Cast<ITargetable>().ToList();
                break;
            case AbilityTarget.AllEnemyCards:
                targets = FindObjectsByType<SnapCard>(FindObjectsSortMode.None).Where(card =>
                    card.ownedPlayer == enemyPlayer && !(card is LocationCard)).Cast<ITargetable>().ToList();
                break;
            case AbilityTarget.AllPlayerPlayedCards:
                List<Location> playerLocations = GetLocations(owner.ownedPlayer);
                foreach (Location location in playerLocations)
                {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<ITargetable>());
                }
                break;
            case AbilityTarget.AllEnemyPlayedCards:
                List<Location> enemyLocations = GetLocations(enemyPlayer);
                foreach (Location location in enemyLocations)
                {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<ITargetable>());
                }
                break;
            case AbilityTarget.AllPlayedCards:
                List<Location> allLocations = GetLocations();
                foreach (Location location in allLocations)
                {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<ITargetable>());
                }
                break;
            case AbilityTarget.PlayerDirectLocation:
                targets = new List<ITargetable> { ownerLocation.cardRepresentation as ITargetable };
                break;
            case AbilityTarget.AllPlayerLocation:
                targets = GetLocations(owner.ownedPlayer).Select(location => location.cardRepresentation as ITargetable).Where(card => card != null).ToList();
                break;
            case AbilityTarget.EnemyDirectLocation:
                targets = GetLocations(enemyPlayer).Select(location => location.cardRepresentation as ITargetable).Where(card => card != null).ToList();
                break;
            case AbilityTarget.AllEnemyLocation:
                targets = GetLocations(enemyPlayer).Select(location => location.cardRepresentation as ITargetable).Where(card => card != null).ToList();
                break;
            case AbilityTarget.DirectLocation:
                targets = GetLocations(ownerLocation.position).Select(location => location.cardRepresentation as ITargetable).Where(card => card != null).ToList();
                break;
            case AbilityTarget.AllLocation:
                targets = GetLocations().Select(location => location.cardRepresentation as ITargetable).Where(card => card != null).ToList();
                break;
            case AbilityTarget.NextPlayedCard:
                SnapCard nextPlayedCard = RevealEventHandler.Instance.GetNextPlayedCard(owner.ownedPlayer);
                if (nextPlayedCard != null)
                    targets.Add(nextPlayedCard);
                break;
            case AbilityTarget.TriggeredActionTargets:
                if (triggeredAction == null)
                {
                    targets = new List<ITargetable>();
                }
                else if (triggeredAction is AbilityEffectGA)
                {
                    AbilityEffectGA abilityEffectGA = (AbilityEffectGA)triggeredAction;
                    targets = abilityEffectGA.targets;
                }
                else if (triggeredAction is RevealCardGA revealCardGA)
                {
                    targets = new List<ITargetable> { revealCardGA.card };
                }
                break;
            case AbilityTarget.TriggeredActionSource:
                if (triggeredAction == null)
                {
                    targets = new List<ITargetable>();
                }
                else if (triggeredAction is AbilityEffectGA)
                {
                    AbilityEffectGA abilityEffectGA = (AbilityEffectGA)triggeredAction;
                    targets = new List<ITargetable> { abilityEffectGA.owner };
                }
                break;
            case AbilityTarget.CreatedCard:
                if (triggeredAction == null)
                {
                    targets = new List<ITargetable>();
                }
                else if (triggeredAction is CreateCardGA createCardGA)
                {
                    targets = createCardGA.createdCards.Cast<ITargetable>().ToList();
                }
                else
                {
                    targets = new List<ITargetable>();
                }
                break;
        }
        // Apply additional filters based on targetRange, targetSort, and targetRequirement
        targets = ApplyRangeFilter(targets, targetDefinition, triggeredAction);
        if (targetDefinition.excludeSelf) {
            targets = targets.Where(target => target != owner).ToList();
        }
        return targets;
    }

    public bool IsCardATarget(List<AbilityTargetDefinition> targetDefinitions, ITargetable cardToCheck, SnapCard owner, GameAction triggeredAction = null) {
        var targets = GetTargets(targetDefinitions, owner, triggeredAction);
        return targets.Contains(cardToCheck);
    }

    private List<ITargetable> ApplyRangeFilter(List<ITargetable> targets, AbilityTargetDefinition targetDefinition, GameAction triggeredAction = null) {
        if (targets.Count == 0) {
            return targets;
        }
        targets = ApplySortFilter(targets, targetDefinition.targetSort);
        AbilityTargetRange range = targetDefinition.targetRange;
        switch (range) {
            case AbilityTargetRange.All:
                return targets;
            case AbilityTargetRange.First:
                return targets[0] != null ? new List<ITargetable> { targets[0] } : new List<ITargetable>();
            case AbilityTargetRange.Last:
                return targets.Count > 0 && targets[targets.Count - 1] != null ? new List<ITargetable> { targets[targets.Count - 1] } : new List<ITargetable>();
            case AbilityTargetRange.AllRequirementsMet:
                return ApplyRequirementFilter(targets, targetDefinition.targetRequirement, triggeredAction);
            case AbilityTargetRange.Random:
                Shuffle(targets);
                if (targets.Count > 0) {
                    return new List<ITargetable> { targets[0] };
                }
                return new List<ITargetable>();
            case AbilityTargetRange.RandomRequirementsMet:
                targets = ApplyRequirementFilter(targets, targetDefinition.targetRequirement, triggeredAction);
                Shuffle(targets);
                if (targets.Count > 0) {
                    return new List<ITargetable> { targets[0] };
                }
                return new List<ITargetable>();
            default:
                return targets;
        }
    }

    private List<ITargetable> ApplySortFilter(List<ITargetable> targets, AbilityTargetSort sort) {
        if (sort == AbilityTargetSort.None) {
            return targets;
        }

        var groupedTargets = targets.GroupBy(card => GetSortValue(card, sort)).OrderBy(group => group.Key);
        List<ITargetable> sortedTargets = new List<ITargetable>();

        foreach (var group in groupedTargets) {
            List<ITargetable> shuffledGroup = group.ToList();
            Shuffle(shuffledGroup);
            sortedTargets.AddRange(shuffledGroup);
        }

        return sortedTargets;
    }

    private int GetSortValue(ITargetable card, AbilityTargetSort sort) {
        if (card is SnapCard snapCard) {
            switch (sort) {
                case AbilityTargetSort.Power:
                    return snapCard.GetPower();
                case AbilityTargetSort.BaseCost:
                    return snapCard.GetBaseCost();
                case AbilityTargetSort.CurrentCost:
                    return snapCard.GetCurrentCost();
                case AbilityTargetSort.PlayedOrder:
                    return snapCard.PlayedOrder;
                case AbilityTargetSort.LocationOrder:
                    Location location = snapCard.PlayedLocation;
                    return location != null ? location.cardGroup.MountedCards.IndexOf(snapCard) : 0;
                default:
                    return 0;
            }
        }
        return 0;
    }

    private void Shuffle<T>(IList<T> list) {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private List<ITargetable> ApplyRequirementFilter(List<ITargetable> targets, List<AbilityRequirement> requirements, GameAction triggeredAction = null) {
        foreach (AbilityRequirement requirement in requirements) {
            targets = targets.Where(target => IsRequirementMet(requirement, new List<ITargetable> { target }, triggeredAction)).ToList();
        }
        return targets;
    }

    public bool IsRequirementMet(AbilityRequirement requirement, List<ITargetable> target, GameAction triggeredAction = null) {
        AbilityRequirementType reqType = requirement.ReqType;
        AbilityRequirementComparator reqComparator = requirement.ReqComparator;
        AbilityAmount reqAmount = requirement.ReqAmount;
        int satisfiedCount = 0;
        foreach (ITargetable snapCard in target) {
            if (snapCard is SnapCard card) {
                AbilityAmount targetValue = GetTargetValue(reqType, card);
                if(targetValue.type == AbilityAmountType.Boolean){
                    return targetValue.GetValue<bool>(card, triggeredAction) == reqAmount.GetValue<bool>(card, triggeredAction);
                }
                if (reqType==AbilityRequirementType.HasKeyword) {
                    if (card.HasKeyword(reqAmount.value))
                        satisfiedCount++;
                }
                else if (CompareRequirementValue(card, targetValue, reqAmount, reqComparator, triggeredAction)) {
                    satisfiedCount++;
                }
            }
        }
        if (requirement.ReqCondition == AbilityRequirementCondition.All) {
            return satisfiedCount == target.Count;
        }
        return satisfiedCount > 0;
    }

    bool CompareRequirementValue(SnapCard target, AbilityAmount targetValue, AbilityAmount reqAmount, AbilityRequirementComparator reqComparator, GameAction triggeredAction = null) {
        int targetValueInt = targetValue.GetValue<int>(target, triggeredAction);
        int reqAmountInt = reqAmount.GetValue<int>(target, triggeredAction);
        
        switch (reqComparator) {
            case AbilityRequirementComparator.Equal:
                return targetValue.value == reqAmount.value;
            case AbilityRequirementComparator.NotEqual:
                return targetValue.value != reqAmount.value;
            case AbilityRequirementComparator.Greater:
                return targetValueInt > reqAmountInt;
            case AbilityRequirementComparator.Less:
                return targetValueInt < reqAmountInt;
            case AbilityRequirementComparator.GEQ:
                return targetValueInt >= reqAmountInt;
            case AbilityRequirementComparator.LEQ:
                return targetValueInt <= reqAmountInt;
            case AbilityRequirementComparator.Contains:
                return targetValue.value.Contains(reqAmount.value);
            case AbilityRequirementComparator.DoesNotContain:
                return !targetValue.value.Contains(reqAmount.value);
            default:
                return true;
        }
    }

    public AbilityAmount GetTargetValue(AbilityRequirementType reqType, ITargetable target) {
        return target.GetTargetValue(reqType);
    }

    public Location GetEnemyLocation(Location location) {
        Player enemyPlayer = location.player == Player.Player1 ? Player.Player2 : Player.Player1;
        return FindObjectsByType<Location>(FindObjectsSortMode.None).First(loc => loc.player == enemyPlayer && loc.position == location.position);
    }

    public List<Location> GetLocations(Player player) {
        return FindObjectsByType<Location>(FindObjectsSortMode.None).Where(location => location.player == player).ToList();
    }

    public List<Location> GetLocations() {
        return FindObjectsByType<Location>(FindObjectsSortMode.None).ToList();
    }

    public List<Location> GetLocations(LocationPosition position) {
        return FindObjectsByType<Location>(FindObjectsSortMode.None).Where(location => location.position == position).ToList();
    }

    public Location GetLocation(LocationPosition position, Player player) {
        return FindObjectsByType<Location>(FindObjectsSortMode.None).First(location => location.position == position && location.player == player);
    }
}