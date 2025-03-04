using System.Collections.Generic;
using UnityEngine;

namespace CardHouse
{
    public class CardLocationLayout : CardGroupSettings
    {
        public int CardsPerRow = 2;
        public float MarginalCardOffset = 0.05f;
        Collider2D MyCollider;
        public bool Straighten = true;
        public Transform[] GridPositions;

        void Awake()
        {
            MyCollider = GetComponent<Collider2D>();
            if (MyCollider == null)
            {
                Debug.LogWarningFormat("{0}:{1} needs Collider2D on its game object to function.", gameObject.name, "GridLayout");
            }
        }

        protected override void ApplySpacing(List<Card> cards, SeekerSetList seekerSets)
        {
            if (GridPositions.Length != 4)
            {
                Debug.LogError("Grid positions array must contain exactly 4 positions.");
                return;
            }

            for (int i = 0; i < cards.Count && i < 4; i++)
            {
                var card = cards[i];
                var newPos = GridPositions[i].position;
                var seekerSet = seekerSets?.GetSeekerSetFor(card);
                card.Homing.StartSeeking(newPos, seekerSet?.Homing);
                if (Straighten)
                {
                    card.Turning.StartSeeking(transform.rotation.eulerAngles.z, seekerSet?.Turning);
                }
                card.Scaling.StartSeeking(UseMyScale ? GridPositions[i].lossyScale.y : 1, seekerSet?.Scaling);
            }
        }
    }
}
