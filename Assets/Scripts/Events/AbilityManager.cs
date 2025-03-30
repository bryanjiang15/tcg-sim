using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using CardHouse;

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

    //Call during performance of OnrevealGA
    public void ActivateOnRevealAbility(SnapCard card){
        if (abilities.ContainsKey(card)) {
            foreach (Ability ability in abilities[card]) {
                AbilityTrigger trigger = ability.definition.trigger;
                if (trigger == AbilityTrigger.OnReveal) {
                    List<SnapCard> targets = TargetSystem.Instance.GetTargets(ability.definition.targetDefinition, card);
                    ActionSystem.Instance.AddReaction(ability.getAbilityEffect(targets));
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

    public bool IsAbilityActiveOnPlay(AbilityTrigger trigger) {
        return trigger != AbilityTrigger.InHand && trigger != AbilityTrigger.InDeck;
    }

    public void ActivateAbility(Ability ability) {
        activeAbilities.Add(ability);
        switch(ability.definition.trigger){
            case AbilityTrigger.GameStart:
                // AbilityManager.Instance.ActivateOnRevealAbility(owner);
                break;
            case AbilityTrigger.EndTurn:
                ActionSystem.SubscribeReaction<EndPhaseGA>((endPhaseGA) =>{
                    if(SnapPhaseManager.Instance.GetCurrentPhaseType() == SnapPhaseType.Reveal){
                        ActionSystem.Instance.AddReaction(ability.getAbilityEffect());
                    }
                }, ReactionTiming.PRE);
                break;
            case AbilityTrigger.BeforeCardPlayed:
                ActionSystem.SubscribeReaction<RevealCardGA>((revealCardGA) =>{
                    if(revealCardGA.card == ability.owner){
                        ActionSystem.Instance.AddReaction(ability.getAbilityEffect());
                    }
                }, ReactionTiming.PRE);
                break;
            case AbilityTrigger.AfterCardPlayed:
                ActionSystem.SubscribeReaction<RevealCardGA>((revealCardGA) =>{
                    if(revealCardGA.card != ability.owner){
                        ActionSystem.Instance.AddReaction(ability.getAbilityEffect());
                    }
                }, ReactionTiming.POST);
                break;
                
        }
    }

    public void HandleNextPlayedCardAbility(Ability ability) {
        Ability temporaryAbility = ability;
        temporaryAbility.definition.trigger = AbilityTrigger.AfterCardPlayed;
        temporaryAbility.exhaust = true;
        ActivateAbility(temporaryAbility);
    }

    //GAMEACTION PERFORMER

    private IEnumerator GainPowerPerformer(GainPowerGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            Buff powerBuff = new StatBuff(BuffType.AdditionalPower, action.owner, action.amount.GetValue<int>(action.owner));
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

    private IEnumerator GainCostPerformer(IncreaseCostGA action) {
        List<SnapCard> targets = action.targets;
        foreach (SnapCard target in targets) {
            Buff costBuff = new StatBuff(BuffType.AdditionalCost, action.owner, action.amount.GetValue<int>(action.owner));
            target.ApplyBuff(costBuff);   
        }
        yield return null;
    }

    
}