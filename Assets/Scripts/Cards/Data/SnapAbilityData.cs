using System;
using System.Collections.Generic;
using CardLibrary;
using UnityEngine;

[Serializable]
public class SnapAbilityData
{
    public AbilityTriggerDefinition triggerDefinition;
    public List<Dictionary<string, object>> snapComponentData;
    public string description;

    // Cached List<ISnapBlockDefinition> to avoid repeated reconstruction
    private List<ISnapComponentDefinition> cachedComponentDefinitions;

    // Empty constructor for JSON deserialization
    public SnapAbilityData() { }

    public List<ISnapComponentDefinition> GetBlockDefinitions()
    {
        // Return cached definitions if available
        if (cachedComponentDefinitions != null)
        {
            return cachedComponentDefinitions;
        }

        // Create new definitions and cache them
        cachedComponentDefinitions = CardLibraryDeserializer.ReconstructSnapComponentDefinitions(snapComponentData);
        return cachedComponentDefinitions;
    }

    /// <summary>
    /// Clears the cached block definitions to force recreation
    /// </summary>
    public void ClearCache()
    {
        cachedComponentDefinitions = null;
    }
}