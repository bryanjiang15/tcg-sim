using System.Collections.Generic;

public class PlayerOptionGA : GameAction {
    public Ability ability;
    public PlayerOptionGA(Ability ability) {
        this.ability = ability;
    }
}