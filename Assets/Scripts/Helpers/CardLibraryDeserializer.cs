using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using CardLibrary;
using Newtonsoft.Json.Linq;

namespace CardLibrary
{
    /// <summary>
    /// Helper class for custom deserialization of CardLibraryData with polymorphic snapComponentDefinitions
    /// </summary>
    public static class CardLibraryDeserializer
    {

        /// <summary>
        /// Reconstructs the snapComponentDefinitions list by deserializing each component based on its type
        /// </summary>
        /// <param name="ability">The ability definition to reconstruct</param>
        public static List<ISnapComponentDefinition> ReconstructSnapComponentDefinitions(List<Dictionary<string, object>> components)
        {
            var reconstructedComponents = new List<ISnapComponentDefinition>();
            
            foreach (var component in components)
            {
                if (component == null) continue;
                
                // Get the component type to determine which concrete class to use
                var componentType = (SnapComponentType)System.Enum.Parse(typeof(SnapComponentType), component["componentType"].ToString());
                ISnapComponentDefinition reconstructedComponent = null;
                
                // Deserialize based on the component type
                switch (componentType)
                {
                    case SnapComponentType.Action:
                        reconstructedComponent = JsonConvert.DeserializeObject<SnapActionDefinition>(
                            JsonConvert.SerializeObject(component));
                        break;
                        
                    case SnapComponentType.If:
                        reconstructedComponent = JsonConvert.DeserializeObject<SnapIfDefinition>(
                            JsonConvert.SerializeObject(component));
                        break;
                        
                    case SnapComponentType.Else:
                        reconstructedComponent = JsonConvert.DeserializeObject<SnapElseDefinition>(
                            JsonConvert.SerializeObject(component));
                        break;
                        
                    case SnapComponentType.EndCondition:
                        reconstructedComponent = JsonConvert.DeserializeObject<SnapEndConditionDefinition>(
                            JsonConvert.SerializeObject(component));
                        break;
                        
                    case SnapComponentType.While:
                        reconstructedComponent = JsonConvert.DeserializeObject<SnapWhileDefinition>(
                            JsonConvert.SerializeObject(component));
                        break;
                        
                    case SnapComponentType.Choice:
                        reconstructedComponent = JsonConvert.DeserializeObject<SnapChoiceDefinition>(
                            JsonConvert.SerializeObject(component));
                        break;
                        
                    default:
                        Debug.LogWarning($"Unknown component type: {componentType}");
                        break;
                }
                
                if (reconstructedComponent != null)
                {
                    reconstructedComponents.Add(reconstructedComponent);
                }
            }
            
            return reconstructedComponents;
        }
        
        /// <summary>
        /// Custom deserialization method for AbilityDefinition that handles polymorphic ISnapComponentDefinition objects
        /// </summary>
        /// <param name="abilityJson">The JObject containing the AbilityDefinition data</param>
        /// <returns>A properly deserialized AbilityDefinition object</returns>
        public static AbilityDefinition DeserializeAbilityDefinition(JObject abilityJson)
        {
            var abilityDefinition = new AbilityDefinition();
            
            // Deserialize all properties except snapComponentDefinitions
            if (abilityJson["triggerDefinition"] != null)
                abilityDefinition.triggerDefinition = abilityJson["triggerDefinition"].ToObject<AbilityTriggerDefinition>();
            
            if (abilityJson["description"] != null)
                abilityDefinition.description = abilityJson["description"].ToString();
            
            // Handle snapComponentDefinitions with custom deserialization using CardLibraryDeserializer
            if (abilityJson["snapComponentDefinitions"] != null)
            {
                var componentsArray = abilityJson["snapComponentDefinitions"] as JArray;
                if (componentsArray != null)
                {
                    var componentsList = new List<Dictionary<string, object>>();
                    
                    foreach (var component in componentsArray)
                    {
                        if (component is JObject componentObj)
                        {
                            var componentDict = componentObj.ToObject<Dictionary<string, object>>();
                            componentsList.Add(componentDict);
                        }
                    }
                    
                    // Use the existing CardLibraryDeserializer to reconstruct the components
                    abilityDefinition.snapComponentDefinitions = ReconstructSnapComponentDefinitions(componentsList);
                }
            }
            
            return abilityDefinition;
        }
    }
} 