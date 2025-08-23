# Tag System Design Document

## Overview
The tag system is a flexible way to categorize and group cards, allowing for dynamic effects and interactions. Tags can represent various card attributes like types, colors, subtypes, and conditions.

## Core Components

### 1. Base Tag Class
- Abstract base class that all specific tags must inherit from
- Properties:
  - `name`: The identifier of the tag
- Abstract Methods:
  - `OnTagAdded()`: Called when tag is successfully added
  - `OnTagRemoved()`: Called when tag is successfully removed
- **Note:** The base Tag class does not add itself to a card. Tag management (adding/removing tags) is handled by taggable objects via the `ITaggable` interface.

### 2. ITaggable Interface
Defines the contract for any object that can have tags attached to it (e.g., cards).

```csharp
public interface ITaggable
{
    IReadOnlyCollection<Tag> Tags { get; }
    bool AddTag(Tag tag);
    bool RemoveTag(Tag tag);
    bool HasTag(string tagName);
    Tag GetTag(string tagName);
}
```

- `Tags`: Exposes the current tags on the object.
- `AddTag(Tag tag)`: Attempts to add a tag, returns success/failure.
- `RemoveTag(Tag tag)`: Attempts to remove a tag, returns success/failure.
- `HasTag(string tagName)`: Checks if a tag is present.
- `GetTag(string tagName)`: Retrieves a tag by name.

**Implementation:**
- Any object (such as `SnapCard`) that should be able to have tags must implement `ITaggable`.
- The logic for adding/removing tags, including checks for permanence or removability, is handled in the implementation of `ITaggable`.

### 3. Tag Categories and Behaviors

#### Permanent Tags
- Associated with an object on initialization and cannot be changed or removed.
- Examples:
  - Card Types (Creature, Spell, Equipment)
  - Card Colors (Red, Blue, Green)
  - Base Subtypes (Warrior, Mage, Beast)

#### Temporary Tags
- Can be added and removed, and may have a duration or become permanent once added.
- Examples:
  - Status Effects (Frozen, Poisoned, Enraged)
  - Buffs/Debuffs (Strength Up, Defense Down)
  - Temporary Conditions (Stunned, Silenced)

#### One-Time Tags
- Can be added but not removed.
- Examples:
  - Transformations
  - Permanent Buffs
  - Special States

### 4. Implementation Examples

```csharp
// Example of a taggable card
public class SnapCard : ITaggable
{
    private List<Tag> tags = new List<Tag>();
    public IReadOnlyCollection<Tag> Tags => tags.AsReadOnly();

    public bool AddTag(Tag tag)
    {
        if (tag == null || HasTag(tag.Name)) return false;
        // Add logic for permanent/non-removable tags here
        tags.Add(tag);
        tag.OnTagAdded();
        return true;
    }

    public bool RemoveTag(Tag tag)
    {
        if (tag == null || !tags.Contains(tag)) return false;
        // Add logic for permanent/non-removable tags here
        tags.Remove(tag);
        tag.OnTagRemoved();
        return true;
    }

    public bool HasTag(string tagName) => tags.Any(t => t.Name == tagName);
    public Tag GetTag(string tagName) => tags.FirstOrDefault(t => t.Name == tagName);
}

// Example of a permanent card type tag
public class CreatureTag : Tag
{
    public CreatureTag() : base("Creature") { }
    protected override void OnTagAdded() { /* ... */ }
    protected override void OnTagRemoved() { /* ... */ }
}
```

## System Features

### 1. Tag Management
- Taggable objects maintain a collection of active tags
- Tags are serializable for save/load functionality
- Tags can be permanent, temporary, or one-time
- Safe tag addition and removal with proper validation in the ITaggable implementation

### 2. Tag Interactions
- Tags can interact with other tags on the same object
- Tags can modify object properties and behaviors
- Tags can trigger events when added or removed
- Tags can have dependencies on other tags

### 3. Tag Effects
- Visual effects for tag presence
- Gameplay effects (modifying stats, abilities, etc.)
- Duration-based effects
- Stacking effects
- Permanent modifications

## Implementation Guidelines

### 1. Creating New Tags
1. Create a new class inheriting from `Tag`
2. Implement required abstract methods
3. Define tag-specific behavior
4. Add any necessary properties or methods

### 2. Making Objects Taggable
1. Implement the `ITaggable` interface
2. Use the interface methods to manage tags, ensuring all tag rules are respected
3. Handle edge cases (e.g., attempting to remove permanent tags) in the implementation

### 3. Best Practices
- Keep tag effects modular and focused
- Use events for tag-related notifications
- Consider tag combinations and interactions
- Document tag behavior clearly
- Handle edge cases (e.g., attempting to remove permanent tags)
- Validate tag operations before executing them

### 4. Performance Considerations
- Cache tag effects where possible
- Use object pooling for frequently created/destroyed tags
- Optimize tag checks and updates
- Consider tag collection management for objects with many tags

## Future Considerations

### 1. Potential Features
- Tag duration system
- Tag stacking mechanics
- Tag combination effects
- Tag-based filtering
- Tag-based rules
- Tag priority system
- Tag conflict resolution

### 2. Extensibility
- Plugin system for custom tags
- Tag effect modifiers
- Tag interaction rules
- Custom tag validation rules

## Questions to Consider
1. How should tags interact with the object's base stats?
2. Should tags have priority levels?
3. How to handle conflicting tag effects?
4. Should tags be able to modify other tags?
5. How to handle tag persistence across game sessions?
6. How to handle tag dependencies and prerequisites?
7. How to manage tag combinations and synergies?

## Next Steps
1. Implement basic tag types (Creature, Spell, etc.)
2. Create tag management system using ITaggable
3. Add visual feedback for tags
4. Implement tag interaction system
5. Add tag-based filtering
6. Create tag effect system
7. Implement tag persistence 