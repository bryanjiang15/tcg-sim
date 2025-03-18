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
        locations = FindObjectsOfType<Location>().ToList();
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
    }

    //GAMEACTION PERFORMER
    private IEnumerator RevealCardPerformer(RevealCardGA action) {
        yield return action.card.revealCard();
    }

    public void EnterRevealPhase() {
        var locations = FindObjectsOfType<Location>();
        var p1Locations = locations.Where(location => location.player == Player.Player1);
        var p2Locations = locations.Where(location => location.player == Player.Player2);

        StartCoroutine(ProcessCardEvents());
    }

    private IEnumerator ProcessCardEvents() {
        Player currentPlayer = Player.Player1;
        foreach (SnapCard card in PlayedCards[currentPlayer]) {
            while(ActionSystem.Instance.IsPerforming) yield return null;
            ActionSystem.Instance.Perform(new RevealCardGA(card));
        }
        while(ActionSystem.Instance.IsPerforming) yield return null;
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
}