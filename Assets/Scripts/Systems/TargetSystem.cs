using System.Collections.Generic;
using System.Linq;
using CardHouse;
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

    public List<SnapCard> GetTargets(List<AbilityTargetDefinition> targetDefinitions, SnapCard owner, GameAction triggeredAction = null) {
        List<SnapCard> targets = new List<SnapCard>();
        foreach (AbilityTargetDefinition targetDefinition in targetDefinitions) {
            targets.AddRange(GetTargets(targetDefinition, owner, triggeredAction));
        }
        return targets;
    }

    public List<SnapCard> GetTargets(AbilityTargetDefinition targetDefinition, SnapCard owner, GameAction triggeredAction = null) {
        List<SnapCard> targets = new List<SnapCard>();
        Location ownerLocation = owner.PlayedLocation;
        Player enemyPlayer = owner.ownedPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        switch (targetDefinition.target) {
            case AbilityTarget.Self:
                targets.Add(owner);
                break;
            case AbilityTarget.Deck:
                targets = GroupRegistry.Instance.Get(GroupName.Deck, (int)owner.ownedPlayer).MountedCards.OfType<SnapCard>().ToList();
                break;
            case AbilityTarget.Hand:
                targets = GroupRegistry.Instance.Get(GroupName.Hand, (int)owner.ownedPlayer).MountedCards.OfType<SnapCard>().ToList();
                break;
            case AbilityTarget.EnemyDeck:
                targets = GroupRegistry.Instance.Get(GroupName.Deck, (int)enemyPlayer).MountedCards.OfType<SnapCard>().ToList();
                break;
            case AbilityTarget.EnemyHand:
                targets = GroupRegistry.Instance.Get(GroupName.Hand, (int)enemyPlayer).MountedCards.OfType<SnapCard>().ToList();
                break;
            case AbilityTarget.PlayerDirectLocationCards:
                targets = ownerLocation.cardGroup.MountedCards.OfType<SnapCard>().ToList();
                break;
            case AbilityTarget.EnemyDirectLocationCards:
                Location enemyLocation = GetEnemyLocation(ownerLocation);
                targets = enemyLocation.cardGroup.MountedCards.OfType<SnapCard>().ToList();
                break;
            
            case AbilityTarget.AllPlayerCards:
                List<Location> playerLocations = GetLocations(owner.ownedPlayer);
                foreach (Location location in playerLocations) {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<SnapCard>());
                }
                break;
            case AbilityTarget.AllEnemyCards:
                List<Location> enemyLocations = GetLocations(enemyPlayer);
                foreach (Location location in enemyLocations) {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<SnapCard>());
                }
                break;
            case AbilityTarget.AllBoardCards:
                List<Location> allLocations = GetLocations();
                foreach (Location location in allLocations) {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<SnapCard>());
                }
                break;
            case AbilityTarget.PlayerDirectLocation:
                targets = new List<SnapCard> { ownerLocation.cardRepresentation };
                break;
            case AbilityTarget.AllPlayerLocation:
                targets = GetLocations(owner.ownedPlayer).Select(location => location.cardRepresentation as SnapCard).Where(card => card != null).ToList();
                break;
            case AbilityTarget.EnemyDirectLocation:
                targets = GetLocations(enemyPlayer).Select(location => location.cardRepresentation as SnapCard).Where(card => card != null).ToList();
                break;
            case AbilityTarget.AllEnemyLocation:
                targets = GetLocations(enemyPlayer).Select(location => location.cardRepresentation as SnapCard).Where(card => card != null).ToList();
                break;
            case AbilityTarget.DirectLocation:
                targets = GetLocations(ownerLocation.position).Select(location => location.cardRepresentation as SnapCard).Where(card => card != null).ToList();
                break;
            case AbilityTarget.AllLocation:
                targets = GetLocations().Select(location => location.cardRepresentation as SnapCard).Where(card => card != null).ToList();
                break;
            case AbilityTarget.NextPlayedCard:
                break;
            case AbilityTarget.TriggeredActionTargets:
                if (triggeredAction == null) {
                    targets = new List<SnapCard>();
                }
                else if (triggeredAction is AbilityEffectGA){
                    AbilityEffectGA abilityEffectGA = (AbilityEffectGA)triggeredAction;
                    targets = abilityEffectGA.targets; // Use the targets from the triggered action if available
                }
                break;
                // Add more cases as needed
            case AbilityTarget.TriggeredActionSource:
                if (triggeredAction == null) {
                    targets = new List<SnapCard>();
                }
                else if (triggeredAction is AbilityEffectGA){
                    AbilityEffectGA abilityEffectGA = (AbilityEffectGA)triggeredAction;
                    targets = new List<SnapCard> { abilityEffectGA.owner }; // Use the owner from the triggered action if available
                }
                break;
            case AbilityTarget.CreatedCard:
                if (triggeredAction == null) {
                    targets = new List<SnapCard>();
                }
                else if (triggeredAction is CreateCardGA createCardGA){
                    targets = createCardGA.createdCards; // Use the created cards from the triggered action if available
                }else
                {
                    targets = new List<SnapCard>();
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

    public bool IsCardATarget(List<AbilityTargetDefinition> targetDefinitions, SnapCard cardToCheck, SnapCard owner, GameAction triggeredAction = null) {
        var targets = GetTargets(targetDefinitions, owner, triggeredAction);
        return false;
    }

    private List<SnapCard> ApplyRangeFilter(List<SnapCard> targets, AbilityTargetDefinition targetDefinition, GameAction triggeredAction = null) {
        targets = ApplySortFilter(targets, targetDefinition.targetSort);
        AbilityTargetRange range = targetDefinition.targetRange;
        switch (range) {
            case AbilityTargetRange.All:
                return targets;
            case AbilityTargetRange.First:
                return targets[0] != null ? new List<SnapCard> { targets[0] } : new List<SnapCard>();
            case AbilityTargetRange.Last:
                return targets.Count > 0 && targets[targets.Count - 1] != null ? new List<SnapCard> { targets[targets.Count - 1] } : new List<SnapCard>();
            case AbilityTargetRange.AllRequirementsMet:
                return ApplyRequirementFilter(targets, targetDefinition.targetRequirement, triggeredAction);
            case AbilityTargetRange.Random:
                Shuffle(targets);
                if (targets.Count > 0) {
                    return new List<SnapCard> { targets[0] };
                }
                return new List<SnapCard>();
            case AbilityTargetRange.RandomRequirementsMet:
                targets = ApplyRequirementFilter(targets, targetDefinition.targetRequirement, triggeredAction);
                Shuffle(targets);
                if (targets.Count > 0) {
                    return new List<SnapCard> { targets[0] };
                }
                return new List<SnapCard>();
            default:
                return targets;
        }
    }

    private List<SnapCard> ApplySortFilter(List<SnapCard> targets, AbilityTargetSort sort) {
        if (sort == AbilityTargetSort.None) {
            return targets;
        }

        var groupedTargets = targets.GroupBy(card => GetSortValue(card, sort)).OrderBy(group => group.Key);
        List<SnapCard> sortedTargets = new List<SnapCard>();

        foreach (var group in groupedTargets) {
            List<SnapCard> shuffledGroup = group.ToList();
            Shuffle(shuffledGroup);
            sortedTargets.AddRange(shuffledGroup);
        }

        return sortedTargets;
    }

    private int GetSortValue(SnapCard card, AbilityTargetSort sort) {
        switch (sort) {
            case AbilityTargetSort.Power:
                return card.GetPower();
            case AbilityTargetSort.BaseCost:
                return card.GetBaseCost();
            case AbilityTargetSort.CurrentCost:
                return card.GetCurrentCost();
            case AbilityTargetSort.PlayedOrder:
                return card.PlayedOrder;
            case AbilityTargetSort.LocationOrder:
                Location location = card.PlayedLocation;
                return location != null ? location.cardGroup.MountedCards.IndexOf(card) : 0;
            default:
                return 0;
        }
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

    private List<SnapCard> ApplyRequirementFilter(List<SnapCard> targets, List<AbilityRequirement> requirements, GameAction triggeredAction = null) {
        // Implement logic to filter targets based on requirements
        foreach (AbilityRequirement requirement in requirements) {
            targets = targets.Where(target => IsRequirementMet(requirement, new List<SnapCard> { target }, triggeredAction)).ToList();
        }
        return targets;
    }

    public bool IsRequirementMet(AbilityRequirement requirement, List<SnapCard> target, GameAction triggeredAction = null) {
        AbilityRequirementType reqType = requirement.ReqType;
        AbilityRequirementComparator reqComparator = requirement.ReqComparator;
        AbilityAmount reqAmount = requirement.ReqAmount;
        int satisfiedCount = 0;
        foreach (SnapCard snapCard in target) {
            AbilityAmount targetValue = GetTargetValue(reqType, snapCard);
            if(targetValue.type == AbilityAmountType.Boolean){
                return targetValue.GetValue<bool>(snapCard, triggeredAction) == reqAmount.GetValue<bool>(snapCard, triggeredAction);
            }
            if (reqType==AbilityRequirementType.HasKeyword) {
                if (snapCard.HasKeyword(reqAmount.value))  // Assuming reqAmount.value is a string representing the keyword
                    satisfiedCount++;
            }
            else if (CompareRequirementValue(snapCard, targetValue, reqAmount, reqComparator, triggeredAction)) {
                satisfiedCount++;
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

    public AbilityAmount GetTargetValue(AbilityRequirementType reqType, SnapCard target) {
        switch (reqType) {
            case AbilityRequirementType.Power:
                return new AbilityAmount { type = AbilityAmountType.Constant, value = target.GetPower().ToString() };
            case AbilityRequirementType.Cost:
                return new AbilityAmount { type = AbilityAmountType.Constant, value = target.GetBaseCost().ToString() };
            // case AbilityRequirementType.NumberOfCards:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = targets.Count.ToString() };
            // case AbilityRequirementType.CurrentTurn:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = TurnManager.Instance.CurrentTurn.ToString() };
            // case AbilityRequirementType.CurrentMaxEnergy:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = targets.GetCurrentMaxEnergy().ToString() };
            // case AbilityRequirementType.LocationPowerDifference:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = targets.GetPowerDifference().ToString() };
            // case AbilityRequirementType.HasKeyword:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = targets.HasKeyword().ToString() };
            // case AbilityRequirementType.IsCreated:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = targets.IsCreated().ToString() };
            // case AbilityRequirementType.CardName:
            //     return new AbilityAmount { type = AbilityAmountType.Constant, value = targets.GetCardName().ToString() };
            // case AbilityRequirementType.BuffPresent:
            case AbilityRequirementType.LocationFull:
                if (target is LocationCard locationCard) {
                    return new AbilityAmount { type = AbilityAmountType.Boolean, value = locationCard.IsFull().ToString() };
                }
                return new AbilityAmount { type = AbilityAmountType.Constant, value = "false" };
            default:
                return new AbilityAmount { type = AbilityAmountType.Constant, value = "0" };
        }
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
}