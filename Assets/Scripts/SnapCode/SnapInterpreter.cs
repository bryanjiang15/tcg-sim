using System.Collections.Generic;
using UnityEngine;

public class SnapInterpreter {
    
    /// <summary>
    /// Converts an AbilityDefinition into a SnapTrigger with appropriate SnapBlocks
    /// </summary>
    /// <param name="abilityDefinition">The ability definition to convert</param>
    /// <returns>A SnapTrigger containing the converted SnapBlocks</returns>
    public static SnapTrigger InterpretAbility(AbilityDefinition abilityDefinition) {
        SnapTrigger snapTrigger = new SnapTrigger();
        
        // Set the trigger definition
        snapTrigger.trigger = abilityDefinition.triggerDefinition;
        
        // Determine reaction timing based on trigger type
        snapTrigger.reactionTiming = GetReactionTiming(abilityDefinition.triggerDefinition.triggerType);
        
        // Convert the snapBlockDefinitions into SnapActions
        snapTrigger.actions = ConvertToSnapActions(abilityDefinition);

        snapTrigger.activationLocation = AbilityActivationLocation.AllUnexcluded;
        snapTrigger.specifiedActivationLocations = new List<string>();
        
        return snapTrigger;
    }
    
    /// <summary>
    /// Converts an AbilityDefinition's snapBlockDefinitions into a list of SnapBlocks
    /// </summary>
    /// <param name=abilityDefinition">The ability definition to convert</param>
    /// <returns>A list of SnapBlocks representing the ability's effects</returns>
    private static List<ISnapComponent> ConvertToSnapActions(AbilityDefinition abilityDefinition) {
        // Check if snapBlockDefinitions is available and not empty
        if (abilityDefinition.snapComponentDefinitions != null && abilityDefinition.snapComponentDefinitions.Count > 0) {
            return ConvertBlockDefinitionsToSnapActions(abilityDefinition.snapComponentDefinitions);
        } else {
            // Fallback to the old method if snapBlockDefinitions is not available
            List<ISnapComponent> actions = new List<ISnapComponent>();
            SnapAction mainAction = new SnapAction();
            mainAction.effect = abilityDefinition.effect;
            mainAction.amount = abilityDefinition.amount;
            mainAction.targetDefinition = abilityDefinition.targetDefinition;
            actions.Add(mainAction);
            return actions;
        }
    }
    
    /// <summary>
    /// Converts a list of snap block definitions into a list of SnapBlocks
    /// </summary>
    /// <param name=blockDefinitions">The list of snap block definitions to convert</param>
    /// <returns>A list of SnapBlocks representing the block definitions</returns>
    private static List<ISnapComponent> ConvertBlockDefinitionsToSnapActions(List<ISnapComponentDefinition> blockDefinitions) {
        List<ISnapComponent> actions = new List<ISnapComponent>();
        
        if (blockDefinitions == null || blockDefinitions.Count == 0) {
            return actions;
        }
        
        for (int i = 0; i < blockDefinitions.Count; i++) {
            var snapBlockDef = blockDefinitions[i];
            
            switch (snapBlockDef.componentType) {
                case SnapComponentType.Action:
                    if (snapBlockDef is SnapActionDefinition actionDef) {
                        SnapAction action = new SnapAction();
                        action.effect = actionDef.effect;
                        action.amount = actionDef.amount;
                        action.targetDefinition = actionDef.targetDefinition;
                        actions.Add(action);
                    }
                    break;
                case SnapComponentType.If:
                    if (snapBlockDef is SnapIfDefinition ifDef) {
                        var (ifBlocks, elseBlocks, endIndex) = ParseIfElseBlocks(blockDefinitions, i);
                        i = endIndex; // Skip the parsed blocks
                        
                        SnapIfCondition ifCondition = new SnapIfCondition();
                        ifCondition.requirement = ifDef.requirement;
                        ifCondition.requirementTarget = ifDef.requirementTarget;
                        ifCondition.trueActions = ConvertBlockDefinitionsToSnapActions(ifBlocks);
                        ifCondition.falseActions = ConvertBlockDefinitionsToSnapActions(elseBlocks);
                        actions.Add(ifCondition);
                    }
                    break;
                case SnapComponentType.Else:
                    // This should be handled by the If block parsing, but if we encounter it here, skip it
                    break;
                case SnapComponentType.EndCondition:
                    // This should be handled by ParseNestedBlocks, but if we encounter it here, skip it
                    break;
                case SnapComponentType.While:
                    if (snapBlockDef is SnapWhileDefinition whileDef) {
                        // Parse nested blocks for the While condition
                        var nestedBlocks = ParseNestedBlocks(blockDefinitions, i + 1, out int endIndex);
                        i = endIndex; // Skip the parsed blocks
                        
                        SnapWhileCondition whileCondition = new SnapWhileCondition();
                        whileCondition.requirement = whileDef.requirement;
                        whileCondition.loopActions = ConvertBlockDefinitionsToSnapActions(nestedBlocks);
                        actions.Add(whileCondition);
                    }
                    break;
                case SnapComponentType.Choice:
                    if (snapBlockDef is SnapChoiceDefinition choiceDef) {
                        if (choiceDef.choiceDefinition.choiceType == AbilityChoiceType.Card) {
                            SnapChoice<SnapCard> choice = new SnapChoice<SnapCard>(choiceDef.choiceDefinition);
                            actions.Add(choice);
                        }
                        //...
                    }
                    break;
                default:
                    Debug.LogWarning($"Unknown block type: {snapBlockDef.componentType}");
                    break;
            }
        }
        
        return actions;
    }
    
    /// <summary>
    /// Parses nested blocks from a given starting index until an EndCondition is found
    /// </summary>
    /// <param name=blockDefinitions">The list of all block definitions</param>
    /// <param name=startIndex">The index to start parsing from</param>
    /// <param name=endIndex">The index where parsing ended (EndCondition or end of list)</param>
    /// <returns>A list of nested block definitions</returns>
    private static List<ISnapComponentDefinition> ParseNestedBlocks(List<ISnapComponentDefinition> blockDefinitions, int startIndex, out int endIndex) {
        List<ISnapComponentDefinition> nestedBlocks = new List<ISnapComponentDefinition>();
        endIndex = startIndex - 1; // Default to startIndex - 1 if no EndCondition found
        
        for (int i = startIndex; i < blockDefinitions.Count; i++) {
            var blockDef = blockDefinitions[i];
            
            if (blockDef.componentType == SnapComponentType.EndCondition) {
                endIndex = i;
                break;
            }
            
            nestedBlocks.Add(blockDef);
            endIndex = i; // Update endIndex to current position
        }
        
        return nestedBlocks;
    }
    
    /// <summary>
    /// Parses If-Else blocks starting from the given index
    /// </summary>
    /// <param name=blockDefinitions">The list of all block definitions</param>
    /// <param name=ifIndex">The index of the If block</param>
    /// <returns>A tuple containing (ifBlocks, elseBlocks, endIndex)</returns>
    private static (List<ISnapComponentDefinition> ifBlocks, List<ISnapComponentDefinition> elseBlocks, int endIndex) ParseIfElseBlocks(List<ISnapComponentDefinition> blockDefinitions, int ifIndex) {
        // Check if the next block is an Else or EndCondition
        int nextBlockIndex = ifIndex + 1;
        List<ISnapComponentDefinition> ifBlocks = new List<ISnapComponentDefinition>();
        List<ISnapComponentDefinition> elseBlocks = new List<ISnapComponentDefinition>();
        int finalEndIndex = ifIndex;
        
        if (nextBlockIndex < blockDefinitions.Count) {
            var nextBlock = blockDefinitions[nextBlockIndex];
            
            if (nextBlock.componentType == SnapComponentType.Else) {
                // If followed by Else: Else acts as end condition for If, then parse Else blocks until EndCondition
                finalEndIndex = nextBlockIndex; // Else acts as the end for If blocks
                
                // Parse Else blocks until EndCondition
                elseBlocks = ParseNestedBlocks(blockDefinitions, nextBlockIndex + 1, out finalEndIndex);
            } else if (nextBlock.componentType == SnapComponentType.EndCondition) {
                // If followed by EndCondition: no blocks for If, no Else
                finalEndIndex = nextBlockIndex;
            } else {
                // If followed by other blocks: parse until EndCondition or Else
                ifBlocks = ParseNestedBlocks(blockDefinitions, nextBlockIndex, out finalEndIndex);
                
                // Check if there's an Else after the parsed blocks
                if (finalEndIndex + 1 < blockDefinitions.Count && 
                    blockDefinitions[finalEndIndex + 1].componentType == SnapComponentType.Else) {
                    // Parse else blocks
                    elseBlocks = ParseNestedBlocks(blockDefinitions, finalEndIndex + 2, out finalEndIndex);
                }
            }
        }
        
        return (ifBlocks, elseBlocks, finalEndIndex);
    }
    
    /// <summary>
    /// Determines the appropriate reaction timing for a given trigger type
    /// </summary>
    /// <param name="triggerType">The type of trigger</param>
    /// <returns>The appropriate ReactionTiming</returns>
    private static ReactionTiming GetReactionTiming(AbilityTriggerType triggerType) {
        switch (triggerType) {
            case AbilityTriggerType.BeforeCardPlayed:
                return ReactionTiming.PRE;
            case AbilityTriggerType.AfterCardPlayed:
            case AbilityTriggerType.AfterAbilityTriggered:
                return ReactionTiming.POST;
            case AbilityTriggerType.OnReveal:
                return ReactionTiming.POST;
            case AbilityTriggerType.Destroyed:
            case AbilityTriggerType.Discarded:
            case AbilityTriggerType.Moved:
                return ReactionTiming.POST;
            case AbilityTriggerType.EndTurn:
            case AbilityTriggerType.StartTurn:
                return ReactionTiming.PRE;
            case AbilityTriggerType.GameStart:
            case AbilityTriggerType.EndGame:
                return ReactionTiming.POST;
            case AbilityTriggerType.Ongoing:
            case AbilityTriggerType.InHand:
            case AbilityTriggerType.InDeck:
            case AbilityTriggerType.Activate:
            case AbilityTriggerType.OnCreated:
            case AbilityTriggerType.None:
            default:
                return ReactionTiming.POST;
        }
    }
    
    /// <summary>
    /// Validates that an AbilityDefinition can be properly converted to SnapBlocks
    /// </summary>
    /// <param name="abilityDefinition">The ability definition to validate</param>
    /// <returns>True if the ability definition is valid for conversion</returns>
    public static bool ValidateAbilityDefinition(AbilityDefinition abilityDefinition) {
        if (abilityDefinition == null) {
            Debug.LogError("AbilityDefinition is null");
            return false;
        }
        
        if (abilityDefinition.triggerDefinition == null) {
            Debug.LogError("AbilityDefinition triggerDefinition is null");
            return false;
        }
        
        // Check if using snapBlockDefinitions or legacy approach
        if (abilityDefinition.snapComponentDefinitions != null && abilityDefinition.snapComponentDefinitions.Count > 0) {
            // Validate snapBlockDefinitions
            foreach (var snapBlockDef in abilityDefinition.snapComponentDefinitions) {
                if (snapBlockDef == null) {
                    Debug.LogError("SnapBlockDefinition is null");
                    return false;
                }
                
                // Validate Action blocks specifically
                if (snapBlockDef.componentType == SnapComponentType.Action && snapBlockDef is SnapActionDefinition actionDef) {
                    if (actionDef.targetDefinition == null || actionDef.targetDefinition.Count == 0) {
                        Debug.LogError("AbilityActionDefinition targetDefinition is null or empty");
                        return false;
                    }
                    
                    if (actionDef.amount == null) {
                        Debug.LogError("AbilityActionDefinition amount is null");
                        return false;
                    }
                    
                    // Validate that the effect type is supported
                    if (!EffectMapper.AbilityEffectTypeMap.ContainsKey(actionDef.effect)) {
                        Debug.LogError($"Unsupported effect type: {actionDef.effect}");
                        return false;
                    }
                }
            }
        } else {
            // Legacy validation for old approach
            if (abilityDefinition.targetDefinition == null || abilityDefinition.targetDefinition.Count == 0) {
                Debug.LogError("AbilityDefinition targetDefinition is null or empty");
                return false;
            }
            
            if (abilityDefinition.amount == null) {
                Debug.LogError("AbilityDefinition amount is null");
                return false;
            }
            
            // Validate that the effect type is supported
            if (!EffectMapper.AbilityEffectTypeMap.ContainsKey(abilityDefinition.effect)) {
                Debug.LogError($"Unsupported effect type: {abilityDefinition.effect}");
                return false;
            }
        }
        
        return true;
    }
}