using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardHouse;

public class RevealEventHandler : MonoBehaviour {
    private static RevealEventHandler Instance;

    public float EventDelay = 0.5f;

    private Stack<IEnumerator> eventStack = new Stack<IEnumerator>();

    private void Awake() {
        Instance = this;
    }

    public void EnterRevealPhase(CardGroup location) {
        foreach (var card in location.MountedCards) {
            if (card is SnapCard snapCard && !snapCard.revealed) {
                eventStack.Push(snapCard.revealCard());
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

    public void PushEvent(IEnumerator coroutine) {
        eventStack.Push(coroutine);
    }
}