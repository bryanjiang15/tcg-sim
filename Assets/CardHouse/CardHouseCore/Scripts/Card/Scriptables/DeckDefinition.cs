using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardHouse
{
    [CreateAssetMenu(menuName = "CardHouse/Deck Definition")]
    [Serializable]
    public class DeckDefinition : ScriptableObject
    {
        public Sprite CardBackArt;
        public List<SnapCardDefinition> CardCollection;
    }
}
