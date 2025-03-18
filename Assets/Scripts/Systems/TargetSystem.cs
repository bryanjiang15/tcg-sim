using System.Collections.Generic;
using System.Linq;
using CardHouse;
using UnityEngine;

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

    public List<SnapCard> GetTargets(AbilityTargetDefinition targetDefinition, SnapCard owner) {
        List<SnapCard> targets = new List<SnapCard>();
        Location ownerLocation = owner.PlayedLocation;
        Player enemyPlayer = owner.ownedPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        switch (targetDefinition.target) {
            case AbilityTarget.Self:
                if (!targetDefinition.excludeSelf) { 
                    targets.Add(owner);
                }
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
                Location enemyLocation = FindObjectsOfType<Location>().First(location => location.player != owner.ownedPlayer && location.position == ownerLocation.position);
                targets = enemyLocation.cardGroup.MountedCards.OfType<SnapCard>().ToList();
                break;
            
            case AbilityTarget.AllPlayerCards:
                List<Location> playerLocations = FindObjectsOfType<Location>().Where(location => location.player == owner.ownedPlayer).ToList();
                foreach (Location location in playerLocations) {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<SnapCard>());
                }
                break;
            case AbilityTarget.AllEnemyCards:
                List<Location> enemyLocations = FindObjectsOfType<Location>().Where(location => location.player != owner.ownedPlayer).ToList();
                foreach (Location location in enemyLocations) {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<SnapCard>());
                }
                break;
            case AbilityTarget.AllBoardCards:
                List<Location> allLocations = FindObjectsOfType<Location>().ToList();
                foreach (Location location in allLocations) {
                    targets.AddRange(location.cardGroup.MountedCards.OfType<SnapCard>());
                }
                break;
                // Add more cases as needed
        }

        // Apply additional filters based on targetRange, targetSort, and targetRequirement
        targets = ApplyRangeFilter(targets, targetDefinition);
        return targets;
    }

    private List<SnapCard> ApplyRangeFilter(List<SnapCard> targets, AbilityTargetDefinition targetDefinition) {
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
                return ApplyRequirementFilter(targets, targetDefinition.targetRequirement);
            case AbilityTargetRange.Random:
                Shuffle(targets);
                if (targets.Count > 0) {
                    return new List<SnapCard> { targets[0] };
                }
                return new List<SnapCard>();
            case AbilityTargetRange.RandomRequirementsMet:
                targets = ApplyRequirementFilter(targets, targetDefinition.targetRequirement);
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

    private List<SnapCard> ApplyRequirementFilter(List<SnapCard> targets, List<AbilityRequirement> requirements) {
        // Implement logic to filter targets based on requirements
        return targets;
    }
}