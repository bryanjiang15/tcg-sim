using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour {
    public static AbilityManager Instance;

    private List<Ability> abilities = new List<Ability>();

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        List<Location> locations = new List<Location>(FindObjectsOfType<Location>());
        foreach (Location location in locations) {
            location.cardGroup.OnGroupChanged.AddListener(CacheAbilities);
            //TODO add hand, deck, discard pile, destroyed pile
        }
    }

    private void CacheAbilities() {
        abilities = new List<Ability>(FindObjectsOfType<Ability>());
    }

    public List<Ability> GetAbilities() {
        return abilities;
    }

    public List<Ability> GetAbilitiesByTrigger(AbilityTrigger trigger) {
        return abilities.FindAll(ability => ability.trigger == trigger);
    }
}