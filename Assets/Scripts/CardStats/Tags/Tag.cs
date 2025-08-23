using UnityEngine;
using System;

/**
 * A tag is a way to categorize a card.
 * It is used to group cards together and to apply effects to them.
 * Tags can be card types, card colors, subtypes/archetypes, conditions/abilities, etc.
 * parameters:
 * - statName: the name of the tag
 * - statValue: bit value of the tag status: 0 = inactive, 1 = active
 * - isPermanent: whether the tag is permanent or not
 */
[Serializable]
public abstract class Tag : CardStat
{
    protected bool isPermanent {get; private set;}


    protected Tag(string name, bool isPermanent, bool isActive) : base(name, isActive ? 1 : 0)
    {
        this.isPermanent = isPermanent;
    }

    /// <summary>
    /// Called when the tag is successfully added to an object
    /// </summary>
    protected abstract void OnTagAdded();

    /// <summary>
    /// Called when the tag is successfully removed from an object
    /// </summary>
    protected abstract void OnTagRemoved();
}


