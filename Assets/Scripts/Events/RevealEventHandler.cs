using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardHouse;
using System.Linq;
using System;

public class RevealEventHandler : MonoBehaviour {
    public static RevealEventHandler Instance;

    public float EventDelay = 0.5f;

    private Dictionary<Player, List<SnapCard>> PlayedCards = new Dictionary<Player, List<SnapCard>>();
    private List<Location> locations = new List<Location>();

    private void Awake() {
        Instance = this;
        locations = FindObjectsByType<Location>(FindObjectsSortMode.None).ToList();
        foreach (var location in locations) {
            location.cardGroup.OnCardMounted.AddListener((Card card) => {
                if (card is SnapCard snapCard && SnapPhaseManager.Instance.GetCurrentPhaseType().Equals(SnapPhaseType.Preparation)) 
                    ReadyPlayedCard(location, snapCard);
            });

            location.cardGroup.OnCardUnmounted.AddListener((Card card) => {
                if (card is SnapCard snapCard && SnapPhaseManager.Instance.GetCurrentPhaseType().Equals(SnapPhaseType.Preparation)) 
                    UnreadyPlayedCard(location, snapCard);
            });
        }

        ActionSystem.AttachPerformer<RevealCardGA>(RevealCardPerformer);
        PlayedCards.Add(Player.Player1, new List<SnapCard>());
        PlayedCards.Add(Player.Player2, new List<SnapCard>());
    }

    //GAMEACTION PERFORMER
    private IEnumerator RevealCardPerformer(RevealCardGA action) {
        yield return action.card.revealCard(action.IsCardPlayed);
    }

    public void EnterRevealPhase() {
        var locations = FindObjectsByType<Location>(FindObjectsSortMode.None);
        var p1Locations = locations.Where(location => location.player == Player.Player1);
        var p2Locations = locations.Where(location => location.player == Player.Player2);

        StartCoroutine(ProcessCardEvents());
    }

    private IEnumerator ProcessCardEvents() {
        Player currentPlayer = Player.Player1;
        while (PlayedCards[currentPlayer].Count > 0) {
            while (ActionSystem.Instance.IsPerforming || AbilityManager.Instance.AbilityChain.Count > 0) yield return null;
            SnapCard card = PlayedCards[currentPlayer][0];
            PlayedCards[currentPlayer].RemoveAt(0);
            ActionSystem.Instance.Perform(new RevealCardGA(card));
        }
        PlayedCards[currentPlayer].Clear();
        while(ActionSystem.Instance.IsPerforming || AbilityManager.Instance.AbilityChain.Count > 0) yield return null;
        SnapPhaseManager.Instance.NextPhase();
    }

    void ReadyPlayedCard(Location location, SnapCard card) {
        
        if (PlayedCards.ContainsKey(location.player)) {
            PlayedCards[location.player].Add(card);
        }else{
            PlayedCards.Add(location.player, new List<SnapCard>());
            PlayedCards[location.player].Add(card);
        }
    }

    void UnreadyPlayedCard(Location location, SnapCard card) {
        if (PlayedCards.ContainsKey(location.player)) {
            PlayedCards[location.player].Remove(card);
        }
    }

    public SnapCard GetNextPlayedCard(Player player) {
        Debug.Log($"number of played cards: {PlayedCards[player].Count}");
        if (PlayedCards.ContainsKey(player) && PlayedCards[player].Count > 0) {
            return PlayedCards[player][0];
        }
        return null;
    }
}