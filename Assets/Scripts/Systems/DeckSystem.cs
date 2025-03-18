using System.Collections;
using CardHouse;
using UnityEngine;

public class DeckSystem : MonoBehaviour {
    [SerializeField] CardTransferOperator p1DrawOperator;
    [SerializeField] CardTransferOperator p2DrawOperator;
    [SerializeField] float drawCardDelay = 0.5f;

    [SerializeField] CardGroup handGroupP1;
    [SerializeField] CardGroup handGroupP2;
    [SerializeField] CardGroup deckGroupP1;
    [SerializeField] CardGroup deckGroupP2;
    int deckReadyVote = 0;

    private void OnEnable() {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
        ActionSystem.SubscribeReaction<RefillEnergyGA>(TurnStartDrawCardReaction, ReactionTiming.POST);
    }

    private void OnDisable() {
        ActionSystem.DetachPerformer<DrawCardGA>();
        ActionSystem.UnsubscribeReaction<RefillEnergyGA>(TurnStartDrawCardReaction, ReactionTiming.POST);
    }

    /*GAMEACTION PERFORMERS*/
    private IEnumerator DrawCardPerformer(DrawCardGA action) {
        for (int i = 0; i < action.numberOfCards; i++) {
            if (action.player == Player.Player1) {
                p1DrawOperator.Activate();
                SnapCard drawnCard = handGroupP1.MountedCards[handGroupP1.MountedCards.Count - 1] as SnapCard;
                drawnCard.OnCardDrawnReaction();
                
            } else {
                p2DrawOperator.Activate();
            }
            yield return new WaitForSeconds(drawCardDelay);
        }
    }
    /*GAMEACTION SUBSCRIBER*/
    private void TurnStartDrawCardReaction(RefillEnergyGA action) {
        if (SnapPhaseManager.Instance.GetCurrentPhaseType() == SnapPhaseType.Preparation) {
            ActionSystem.Instance.AddReaction(new DrawCardGA(1, Player.Player1));
            ActionSystem.Instance.AddReaction(new DrawCardGA(1, Player.Player2));
        }
    }
    /*END*/

    public void DrawCard(Player player, int numberOfCards, bool initialDraw = false) {
        if (ActionSystem.Instance.IsPerforming) return;
        ActionSystem.Instance.Perform(new DrawCardGA(numberOfCards, player, true));
    }

    private IEnumerator DrawInitialHand()
    {
        while (ActionSystem.Instance.IsPerforming) {
            yield return null;
        }
        DrawCard(Player.Player1, 3, true);

        while (ActionSystem.Instance.IsPerforming) {
            yield return null;
        }
        DrawCard(Player.Player2, 3, true);

        while (ActionSystem.Instance.IsPerforming) {
            yield return null;
        }
        SnapPhaseManager.Instance.NextPhase();
    }

    public void DeckReady() {
        deckReadyVote++;
        if (deckReadyVote == 2 && SnapPhaseManager.Instance.GetCurrentPhaseType() == SnapPhaseType.Start) {
            StartCoroutine(DrawInitialHand());
        }
    }
}