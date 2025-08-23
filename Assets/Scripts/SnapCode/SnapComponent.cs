using System.Collections;

public interface ISnapComponent{
    public IEnumerator Execute(Ability ability, GameAction triggeredAction = null, ITargetable triggeredTarget = null);

}