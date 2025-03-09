using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardHouse;
using System.Linq;

public class RevealEventHandler : MonoBehaviour {
    public static RevealEventHandler Instance;

    public float EventDelay = 0.5f;

    private Stack<IEnumerator> eventStack = new Stack<IEnumerator>();

    private void Awake() {
        Instance = this;
    }

    public void EnterRevealPhase() {
        var locations = FindObjectsOfType<Location>();
        var p1Locations = locations.Where(location => location.player == Player.Player1);
        var p2Locations = locations.Where(location => location.player == Player.Player2);
        foreach (var location in p1Locations) {
            foreach (var card in location.cardGroup.MountedCards) {
                if (card is SnapCard snapCard && !snapCard.revealed) {
                    eventStack.Push(snapCard.revealCard());
                }
            }
        }
        foreach (var location in p2Locations) {
            foreach (var card in location.cardGroup.MountedCards) {
                if (card is SnapCard snapCard && !snapCard.revealed) {
                    eventStack.Push(snapCard.revealCard());
                }
            }
        }
        StartCoroutine(ProcessEventStack());
    }

    private IEnumerator ProcessEventStack() {
        while (eventStack.Count > 0) {
            var currentEvent = eventStack.Pop();
            yield return StartCoroutine(currentEvent);
            yield return new WaitForSeconds(EventDelay); // Add a delay between each event
        }
    }

    public void triggerAbilityFromEvent(AbilityTrigger trigger, Player curPlayer){
        List<Ability> abilities = new List<Ability>(FindObjectsOfType<Ability>());
    }

    public void PushEvent(IEnumerator coroutine) {
        eventStack.Push(coroutine);
    }
}