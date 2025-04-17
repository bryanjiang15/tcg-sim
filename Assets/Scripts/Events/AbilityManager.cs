using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using CardHouse;
using Unity.Mathematics;
using UnityEditor;

public class AbilityManager : MonoBehaviour {
    public static AbilityManager Instance;

    //AbilityManager: sets up reactions for all abilities in game.
    //When reacting to game action, make sure any reactions are added in order of Owner, same player, enemy player
    private Dictionary<SnapCard, List<Ability>> abilities = new Dictionary<SnapCard, List<Ability>>();

    //Active abilities: abilities that are currently active on the field: Ongoing abilities, triggered abilities of cards in play, some from hand/deck abilities
    //Non activate abilities: abilities not activatd in hand/deck that are not on the field
    private List<Ability> activeAbilities = new List<Ability>();
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        List<Location> locations = new List<Location>(FindObjectsByType<Location>(FindObjectsSortMode.None));
        ActionSystem.AttachPerformer<GainPowerGA>(GainPowerPerformer);
        ActionSystem.AttachPerformer<DestroyCardGA>(DestroyCardPerformer);
        ActionSystem.AttachPerformer<DiscardCardGA>(DiscardCardPerformer);
        ActionSystem.AttachPerformer<IncreaseCostGA>(GainCostPerformer);
        ActionSystem.AttachPerformer<CreateCardInLocationGA>(CreateCardInLocationPerformer);
        ActionSystem.AttachPerformer<SetPowerGA>(SetPowerPerformer);
    }

    public Dictionary<SnapCard, List<Ability>> GetAbilities() {
        return abilities;
    }

    public void RegisterAbility(Ability ability) {
        if (!abilities.ContainsKey(ability.owner)) {
            abilities[ability.owner] = new List<Ability>();
        }
        abilities[ability.owner].Add(ability);
    }

    public void UnregisterAbility(Ability ability) {
        if (abilities.ContainsKey(ability.owner)) {
            abilities[ability.owner].Remove(ability);
            if (abilities[ability.owner].Count == 0) {
                abilities.Remove(ability.owner);
            }
        }
    }

    public void SetUpAbilities(SnapCardDefinition snapCardDefinition, SnapCard owner){
        Ability lastAbility = null;
        for(int i = 0; i < snapCardDefinition.abilities.Count(); i++){
            AbilityDefinition abilityDefinition = snapCardDefinition.abilities[i];
            Ability ability = new Ability();
            ability.SetOwner(owner);
            ability.SetUpDefinition(abilityDefinition);
            if(ability.definition.triggerDefinition.trigger == AbilityTrigger.AfterAbilityTriggered){
                if(lastAbility != null){
                    SubscribeAbilityChain(lastAbility, ability);
                }else{
                    Debug.LogError($"No last ability found for ability {i} on card {owner.name}");
                }
            }
            RegisterAbility(ability);
            lastAbility = ability;
        }
    }

    //Call during performance of OnrevealGA
    public void ActivateOnRevealAbility(SnapCard card){
        if (abilities.ContainsKey(card)) {
            foreach (Ability ability in abilities[card]) {
                AbilityTrigger trigger = ability.definition.triggerDefinition.trigger;
                if (trigger == AbilityTrigger.OnReveal) {
                    TriggerAbilityReaction(card, ability); // Trigger the reaction for OnReveal abilities
                } else if (trigger == AbilityTrigger.Ongoing) {
                    OngoingAbility ongoingAbility = new OngoingAbility(ability);
                    activeAbilities.Add(ongoingAbility);
                    ongoingAbility.Activate();
                } else if (IsAbilityActiveOnPlay(trigger)) {
                    ActivateAbility(ability);
                }
            }
        }
    }

    public void TriggerAbilityReaction(SnapCard owner, Ability ability, List<SnapCard> targets=null, GameAction triggeredAction = null) {
        if (triggeredAction != null) {
            //Check if the triggered target is valid for the ability
            List<AbilityTargetDefinition> triggeredTargetDefinitions = ability.definition.triggerDefinition.triggeredTarget;
            if (triggeredTargetDefinitions != null && triggeredTargetDefinitions.Count > 0) {
                List<SnapCard> triggeredTargets = TargetSystem.Instance.GetTargets(triggeredTargetDefinitions, owner, triggeredAction: triggeredAction);
                if (triggeredTargets.Count == 0) {
                    Debug.Log($"Triggered action does not have valid targets for ability: {ability.owner.name}, {ability.definition.triggerDefinition.triggeredTarget[0].target}");

                    return;
                }
            }
        }

        if (ability.definition.activationRequirements != null && ability.definition.activationRequirements.Count > 0) {
            // Check if activation requirements are met
            List<AbilityTargetDefinition> activationTargetDefinitions = new List<AbilityTargetDefinition> { ability.definition.activationRequirementTargets };
            List<SnapCard> ActivationReqTargets = TargetSystem.Instance.GetTargets(activationTargetDefinitions, owner, triggeredAction: triggeredAction);
            foreach (var requirement in ability.definition.activationRequirements) {
                if (!TargetSystem.Instance.IsRequirementMet(requirement, ActivationReqTargets)) {
                    Debug.Log($"Activation requirements not met for ability: {ability.owner.stats.card_name} - {ability.definition.description}");
                    return; // Do not activate if requirements are not met
                }
            }
        }     
        ActionSystem.Instance.AddReaction(ability.getAbilityEffect(targets, triggeredAction));
        HandleNextPlayedCardAbility(ability); // Handle things regarding next played card ability
        if (ability.exhaust) {
            ability.UnsubscribeActionCallback();
        }
    }

    public bool IsAbilityActiveOnPlay(AbilityTrigger trigger) {
        return trigger != AbilityTrigger.InHand && trigger != AbilityTrigger.InDeck;
    }

    public void ActivateAbility(Ability ability) {
        activeAbilities.Add(ability);
        switch(ability.definition.triggerDefinition.trigger) {
            case AbilityTrigger.GameStart:
                // AbilityManager.Instance.ActivateOnRevealAbility(owner);
                break;
            case AbilityTrigger.EndTurn:
                Action<GameAction> endPhaseAction = (endPhaseGA) => {
                    if (SnapPhaseManager.Instance.GetCurrentPhaseType() == SnapPhaseType.Reveal) {
                        TriggerAbilityReaction(ability.owner, ability, triggeredAction: endPhaseGA);
                    }
                };
                ActionSystem.SubscribeReaction<EndPhaseGA>(endPhaseAction, ReactionTiming.PRE);
                ability.SetUnsubscribeActionCallback((endPhaseGA) => {
                    ActionSystem.UnsubscribeReaction<EndPhaseGA>(endPhaseAction, ReactionTiming.PRE);
                });
                break;
            case AbilityTrigger.Destroyed:
                Action<GameAction> destroyCardAction = (destroyCardGA) => {
                    TriggerAbilityReaction(ability.owner, ability, triggeredAction: destroyCardGA);
                };
                ActionSystem.SubscribeReaction<DestroyCardGA>(destroyCardAction, ReactionTiming.POST);
                ability.SetUnsubscribeActionCallback((destroyCardGA) => {
                    ActionSystem.UnsubscribeReaction<DestroyCardGA>(destroyCardAction, ReactionTiming.POST);
                });
                
                break;
            case AbilityTrigger.BeforeCardPlayed:
                Action<GameAction> beforeCardPlayedAction = (beforeCardPlayedGA) => {
                    TriggerAbilityReaction(ability.owner, ability, triggeredAction: beforeCardPlayedGA);
                };
                ActionSystem.SubscribeReaction<RevealCardGA>(beforeCardPlayedAction, ReactionTiming.PRE);
                ability.SetUnsubscribeActionCallback((beforeCardPlayedGA) => {
                    ActionSystem.UnsubscribeReaction<RevealCardGA>(beforeCardPlayedAction, ReactionTiming.PRE);
                });
                break;
            case AbilityTrigger.AfterCardPlayed:
                Action<GameAction> afterCardPlayedAction = (afterCardPlayedGA) => {
                    TriggerAbilityReaction(ability.owner, ability, triggeredAction: afterCardPlayedGA);
                };
                ActionSystem.SubscribeReaction<RevealCardGA>(afterCardPlayedAction, ReactionTiming.POST);
                ability.SetUnsubscribeActionCallback((afterCardPlayedGA) => {
                    ActionSystem.UnsubscribeReaction<RevealCardGA>(afterCardPlayedAction, ReactionTiming.POST);
                });
                break;
                
        }
    }

    public void HandleNextPlayedCardAbility(Ability ability) {
        if (!RuleSystem.Instance.checkAbilityTargetingNextCard(ability)) return;

        //If nextPlayedCard is not played this turn
        if (TargetSystem.Instance.GetTargets(new List<AbilityTargetDefinition> {
            new AbilityTargetDefinition(AbilityTarget.NextPlayedCard) 
        }, ability.owner).Count != 0) return;

        Ability temporaryAbility = ability;
        temporaryAbility.definition.triggerDefinition.trigger = AbilityTrigger.BeforeCardPlayed;
        temporaryAbility.definition.triggerDefinition.triggeredTarget = new List<AbilityTargetDefinition> {
            new AbilityTargetDefinition(AbilityTarget.AllPlayerCards)
        };
        var nextCardPlayedTarget = new AbilityTargetDefinition(AbilityTarget.TriggeredActionTargets, AbilityTargetRange.First);
        temporaryAbility.definition.targetDefinition = new List<AbilityTargetDefinition> { nextCardPlayedTarget };
        temporaryAbility.exhaust = true;
        ActivateAbility(temporaryAbility);
    }

    public SnapCard GetCard(string cardName) {
        SnapCardDefinition cardDefinition = Resources.Load<SnapCardDefinition>($"ScriptableObjects/SnapCards/{cardName}");
        
        if (cardDefinition == null) {
            Debug.LogError($"SnapCardDefinition with name {cardName} not found.");
            return null;
        }

        GameObject cardPrefab = Resources.Load<GameObject>("Prefab/SnapCard");
        if (cardPrefab == null) {
            Debug.LogError("SnapCard prefab not found.");
            return null;
        }

        GameObject cardObject = Instantiate(cardPrefab);

        SnapCardSetup cardSetup = cardObject.GetComponent<SnapCardSetup>();
        if (cardSetup != null) {
            cardSetup.Apply(cardDefinition);
        } else {
            Debug.LogError("SnapCardSetup component missing on instantiated prefab.");
            Destroy(cardObject);
        }
        return cardObject.GetComponent<SnapCard>();
    }

    private void SubscribeAbilityChain(Ability trigger, Ability activatedAbility) {
        Type abilityEffectType = Ability.AbilityEffectTypeMap[trigger.definition.effect];
        MethodInfo subscribeMethod = typeof(ActionSystem).GetMethod("SubscribeReaction").MakeGenericMethod(abilityEffectType);
        subscribeMethod.Invoke(null, new object[] { (Action<GameAction>)((abilityTriggeredGA) => {
            Debug.Log($"Ability triggered: {abilityTriggeredGA}");
            if (abilityTriggeredGA is AbilityEffectGA abilityEffectGA && abilityEffectGA.ability == trigger) {
                 // Trigger the reaction for the activated ability
                TriggerAbilityReaction(activatedAbility.owner, activatedAbility, triggeredAction: abilityTriggeredGA);
            }
        }), ReactionTiming.POST });
    }

    //GAMEACTION PERFORMER

    private IEnumerator GainPowerPerformer(GainPowerGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            Buff powerBuff = new StatBuff(BuffType.AdditionalPower, action.owner, action.amount.GetValue<int>(action.owner, triggeredAction: action));
            target.ApplyBuff(powerBuff);   
        }
        yield return null;
    }

    private IEnumerator DestroyCardPerformer(DestroyCardGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            if (target.PlayedLocation != null) {
                target.GetComponent<DestroyCardOperator>().Activate();
            }
           
        }
        yield return null;
    }

    private IEnumerator DiscardCardPerformer(DiscardCardGA action) {
        List<SnapCard> targets = action.targets;
        List<SnapCard> cardsInHand = GroupRegistry.Instance.Get(GroupName.Hand, 0).MountedCards.Cast<SnapCard>().ToList();
        cardsInHand.AddRange(GroupRegistry.Instance.Get(GroupName.Hand, 1).MountedCards.Cast<SnapCard>().ToList());
        foreach (SnapCard target in targets) {
            if (cardsInHand.Contains(target)) {
                target.GetComponent<DiscardCardOperator>().Activate();
            }
        }
        yield return null;
    }

    private IEnumerator CreateCardInLocationPerformer(CreateCardInLocationGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            if (!(target is LocationCard)) {
                continue;
            }
            LocationCard locationCard = target as LocationCard;
            // Create a new card in the location
            if (!locationCard.IsFull()) {
                SnapCard newCard = GetCard(action.amount.GetValue<string>(action.owner, triggeredAction: action));
                if (newCard == null) {
                    Debug.LogError($"Failed to create card: {action.amount.GetValue<string>(action.owner, triggeredAction: action)}");
                    continue;
                }
                
                locationCard.location.cardGroup.Mount(newCard);
                action.createdCards.Add(newCard);
                ActionSystem.Instance.AddReaction(new RevealCardGA(newCard, IsCardPlayed: false));
            }
            
        }
        yield return null;
    }

    private IEnumerator GainCostPerformer(IncreaseCostGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            Buff costBuff = new StatBuff(BuffType.AdditionalCost, action.owner, action.amount.GetValue<int>(action.owner, triggeredAction: action));
            target.ApplyBuff(costBuff);   
        }
        yield return null;
    }

    private IEnumerator SetPowerPerformer(SetPowerGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            StatBuff powerBuff = new StatBuff(BuffType.SetPower, action.owner, action.amount.GetValue<int>(action.owner, triggeredAction: action));
            Debug.Log($"Setting power of {target.name} to {powerBuff.amount}");
            target.ApplyBuff(powerBuff);   
        }
        yield return null;
    }

    
}