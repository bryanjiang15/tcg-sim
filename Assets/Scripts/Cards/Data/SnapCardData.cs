using System;
using System.Collections.Generic;
using System.Linq;
using CardLibrary;
using UnityEngine;

[Serializable]
public class SnapCardData
{
    public int cost;
    public int power;
    public string card_name;
    public int series;
    public string artPath; // Store the path to the art asset instead of the Sprite
    public List<SnapAbilityData> abilities; // Store ability IDs

    // Cached CardDefinition to avoid repeated creation
    private SnapCardDefinition cachedDefinition;

    // Constructor to create SnapCardData from SnapCardDefinition
    public SnapCardData(SnapCardDefinition definition)
    {
        cost = definition.cost;
        power = definition.power;
        card_name = definition.card_name;
        series = definition.series;
        artPath = definition.Art != null ? definition.Art.name : string.Empty;
        abilities = new List<SnapAbilityData>();
        
        if (definition.abilities != null)
        {
            foreach (var ability in definition.abilities)
            {
                abilities.Add(ObjectMapper.GetSnapAbilityData(ability));
            }
        }
    }

    // Empty constructor for JSON deserialization
    public SnapCardData() { }

    public SnapCardDefinition getCardDefinition(int card_id)
    {
        // Return cached definition if available
        if (cachedDefinition != null)
        {
            return cachedDefinition;
        }

        // Create new definition and cache it
        cachedDefinition = ScriptableObject.CreateInstance<SnapCardDefinition>();
        cachedDefinition.card_id = card_id;
        cachedDefinition.cost = cost;
        cachedDefinition.power = power;
        cachedDefinition.card_name = card_name;
        cachedDefinition.series = series;
        cachedDefinition.abilities = abilities.Select(ability => ObjectMapper.GetAbilityDefinition(ability)).ToList();
        cachedDefinition.Art = CardLibraryManager.Instance.LoadCardArt(artPath);
        cachedDefinition.artPath = artPath;
        
        return cachedDefinition;
    }

    /// <summary>
    /// Clears the cached definition to force recreation (useful when art is updated)
    /// </summary>
    public void ClearCache()
    {
        if (cachedDefinition != null)
        {
            UnityEngine.Object.DestroyImmediate(cachedDefinition);
            cachedDefinition = null;
        }
    }
} 