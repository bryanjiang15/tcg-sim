using UnityEngine;
using System;

/**
 * A tag is a way to categorize a card.
 * It is used to group cards together and to apply effects to them.
 * Tags can be card types, card colors, subtypes/archetypes, conditions/abilities, etc.
 */
[Serializable]
public abstract class Tag
{
    [SerializeField] protected string name {get; private set;}
    [SerializeField] protected bool isPermanent {get; private set;}
    [SerializeField] protected int tagId {get; private set;}

    protected Tag(string name, bool isPermanent, int tagId)
    {
        this.name = name;
        this.isPermanent = isPermanent;
        this.tagId = tagId;
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


