using System;
using System.Collections.Generic;
using CardLibrary;
using UnityEngine;

[Serializable]
public class SnapCardData
{
    public int cost;
    public int power;
    public string card_name;
    public int series;
    public string artPath; // Store the path to the art asset instead of the Spri te
    public List<AbilityDefinition> abilities; // Store ability IDs

    // Constructor to create SnapCardData from SnapCardDefinition
    public SnapCardData(SnapCardDefinition definition)
    {
        cost = definition.cost;
        power = definition.power;
        card_name = definition.card_name;
        series = definition.series;
        artPath = definition.Art != null ? definition.Art.name : string.Empty;
        abilities = new List<AbilityDefinition>();
        
        if (definition.abilities != null)
        {
            foreach (var ability in definition.abilities)
            {
                abilities.Add(ability);
            }
        }
    }

    // Empty constructor for JSON deserialization
    public SnapCardData() { }

    public SnapCardDefinition getCardDefinition()
    {
        var cardDefinition = ScriptableObject.CreateInstance<SnapCardDefinition>();
        cardDefinition.cost = cost;
        cardDefinition.power = power;
        cardDefinition.card_name = card_name;
        cardDefinition.series = series;
        cardDefinition.abilities = abilities;
        cardDefinition.Art = CardLibraryManager.Instance.LoadCardArt(artPath);
        cardDefinition.artPath = artPath;
        return cardDefinition;
    }
} 