# User-Created Tag System

This system allows users to create, manage, and apply custom tags to cards in your TCG game. It provides a flexible and extensible way to categorize and organize cards with user-defined attributes.

## Features

- **User-Created Tags**: Players can create their own tags with custom names and descriptions
- **Tag Categories**: Organize tags into predefined categories (Card Type, Status, Effect, etc.)
- **Persistence**: Tags are automatically saved and loaded between game sessions
- **Search & Filter**: Find tags by name, description, or category
- **Validation**: Built-in validation for tag names and properties
- **Event System**: Subscribe to tag creation, update, and deletion events
- **Statistics**: Track tag usage and get system statistics

## Setup Instructions

### 1. Add TagRegistry to Your Scene

1. Create an empty GameObject in your scene
2. Add the `TagRegistry` component to it
3. Configure the settings in the inspector:
   - **Save File Name**: Name of the JSON file for persistence (default: "user_tags.json")
   - **Auto Save**: Whether to automatically save tags periodically
   - **Auto Save Interval**: How often to save tags (in seconds)

### 2. Configure Dependencies

The system uses Newtonsoft.Json for serialization. Make sure you have it installed in your Unity project:

1. Open Window > Package Manager
2. Search for "Newtonsoft.Json"
3. Install the package if not already installed

### 3. Implement ITaggable on Your Cards

Your card classes should implement the `ITaggable` interface:

```csharp
public class YourCard : MonoBehaviour, ITaggable
{
    private List<Tag> tags = new List<Tag>();
    
    public IReadOnlyCollection<Tag> Tags => tags.AsReadOnly();
    
    public bool AddTag(Tag tag)
    {
        if (tag == null || HasTag(tag.statName)) return false;
        
        // Add validation logic here (e.g., check for permanent tags)
        tags.Add(tag);
        tag.OnTagAdded();
        return true;
    }
    
    public bool RemoveTag(Tag tag)
    {
        if (tag == null || !tags.Contains(tag)) return false;
        
        // Add validation logic here (e.g., prevent removal of permanent tags)
        tags.Remove(tag);
        tag.OnTagRemoved();
        return true;
    }
    
    public bool HasTag(string tagName) => tags.Any(t => t.statName == tagName);
    public Tag GetTag(string tagName) => tags.FirstOrDefault(t => t.statName == tagName);
    public Tag GetTagById(int tagId) => tags.FirstOrDefault(t => /* implement ID logic */);
}
```

## Usage Examples

### Creating Tags

```csharp
// Create a simple tag
var dragonTag = TagManager.CreateTag("Dragon", "Powerful flying creatures");

// Create a permanent tag
var rareTag = TagManager.CreateTag("Rare", "Hard to find cards", TagCategory.Custom, true);

// Create a status effect tag
var frozenTag = TagManager.CreateTag("Frozen", "Cannot attack or defend", TagCategory.Status, false);
```

### Applying Tags to Cards

```csharp
// Add a tag to a card by name
TagManager.AddTagToObject(yourCard, "Dragon");

// Add a tag to a card by ID
TagManager.AddTagToObject(yourCard, dragonTagId);

// Remove a tag from a card
TagManager.RemoveTagFromObject(yourCard, "Frozen");
```

### Searching and Filtering Tags

```csharp
// Search for tags by name or description
var searchResults = TagManager.SearchTags("dragon");

// Get all tags in a specific category
var statusTags = TagManager.GetTagsByCategory(TagCategory.Status);

// Get available tags for a specific card
var availableTags = TagManager.GetAvailableTagsForObject(yourCard);
```

### Managing Tags

```csharp
// Update a tag
TagManager.Registry.UpdateTag(tagId, newName: "Updated Name", newDescription: "New description");

// Delete a tag
TagManager.Registry.DeleteTag(tagId);

// Get tag statistics
var stats = TagManager.GetTagStatistics();
Debug.Log($"Total tags: {stats.TotalTags}");
```

## Tag Categories

The system includes predefined categories for organizing tags:

- **CardType**: Basic card types (Creature, Spell, Equipment, etc.)
- **CardColor**: Card colors (Red, Blue, Green, etc.)
- **Subtype**: Card subtypes (Warrior, Mage, Beast, etc.)
- **Status**: Status effects (Frozen, Poisoned, Enraged, etc.)
- **Effect**: Buffs, debuffs, and conditions
- **Custom**: User-defined categories

## Event System

Subscribe to tag events to react to changes:

```csharp
private void Start()
{
    TagRegistry.OnTagCreated += OnTagCreated;
    TagRegistry.OnTagUpdated += OnTagUpdated;
    TagRegistry.OnTagDeleted += OnTagDeleted;
    TagRegistry.OnTagsLoaded += OnTagsLoaded;
}

private void OnDestroy()
{
    TagRegistry.OnTagCreated -= OnTagCreated;
    TagRegistry.OnTagUpdated -= OnTagUpdated;
    TagRegistry.OnTagDeleted -= OnTagDeleted;
    TagRegistry.OnTagsLoaded -= OnTagsLoaded;
}

private void OnTagCreated(TagDefinition tagDef)
{
    Debug.Log($"New tag created: {tagDef.Name}");
}
```

## Validation

The system includes built-in validation for tag names:

```csharp
// Check if a tag name is valid
bool isValid = TagManager.IsValidTagName("My Tag");

// Get a suggested name (removes invalid characters)
string suggested = TagManager.GetSuggestedTagName("Invalid/Tag Name");
```

## Persistence

Tags are automatically saved to a JSON file in the application's persistent data path. The file is automatically loaded when the TagRegistry initializes.

### Manual Save/Load

```csharp
// Manually save tags
TagManager.Registry.SaveTags();

// Manually load tags
TagManager.Registry.LoadTags();
```

## Performance Considerations

- The system uses dictionaries for O(1) lookups by name and ID
- Tags are cached in memory for fast access
- Auto-save is configurable to balance performance and data safety
- Large numbers of tags (>1000) may impact performance during searches

## Best Practices

1. **Use Descriptive Names**: Choose clear, descriptive tag names
2. **Categorize Appropriately**: Use the correct category for each tag
3. **Validate User Input**: Always validate tag names before creation
4. **Handle Events**: Subscribe to tag events for UI updates
5. **Backup Data**: Consider backing up the tag save file
6. **Limit Tag Count**: Avoid creating too many tags to maintain performance

## Troubleshooting

### Common Issues

1. **TagRegistry not found**: Make sure TagRegistry is in the scene and properly initialized
2. **Tags not saving**: Check that the application has write permissions to the persistent data path
3. **Performance issues**: Consider reducing auto-save frequency or implementing tag cleanup
4. **Serialization errors**: Ensure Newtonsoft.Json is properly installed

### Debug Information

Enable debug logging to see detailed information about tag operations:

```csharp
// The system automatically logs important operations
// Check the console for debug messages
```

## Extending the System

### Custom Tag Categories

To add new tag categories, modify the `TagCategory` enum:

```csharp
public enum TagCategory
{
    CardType,
    CardColor,
    Subtype,
    Status,
    Effect,
    Custom,
    YourNewCategory  // Add your new category here
}
```

### Custom Tag Behaviors

Create custom tag classes that inherit from `Tag`:

```csharp
public class CustomTag : Tag
{
    public CustomTag(string name, bool isPermanent, bool isActive) 
        : base(name, isPermanent, isActive)
    {
    }
    
    protected override void OnTagAdded()
    {
        // Custom logic when tag is added
    }
    
    protected override void OnTagRemoved()
    {
        // Custom logic when tag is removed
    }
}
```

## API Reference

### TagRegistry

- `CreateTag(name, description, category, isPermanent)` - Creates a new tag
- `UpdateTag(id, newName, newDescription, newCategory, newIsPermanent)` - Updates an existing tag
- `DeleteTag(id)` - Deletes a tag
- `GetTag(name)` - Gets a tag by name
- `GetTagById(id)` - Gets a tag by ID
- `HasTag(name)` - Checks if a tag exists
- `GetTagsByCategory(category)` - Gets all tags in a category
- `SearchTags(searchTerm)` - Searches for tags
- `SaveTags()` - Manually saves tags
- `LoadTags()` - Manually loads tags

### TagManager

- `CreateTag(...)` - Convenience method for creating tags
- `AddTagToObject(taggable, tagName)` - Adds a tag to an object
- `RemoveTagFromObject(taggable, tagName)` - Removes a tag from an object
- `GetAvailableTagsForObject(taggable)` - Gets available tags for an object
- `GetAppliedTagsForObject(taggable)` - Gets applied tags for an object
- `IsValidTagName(name)` - Validates a tag name
- `GetSuggestedTagName(input)` - Gets a suggested tag name
- `GetTagStatistics()` - Gets system statistics

### Events

- `OnTagCreated` - Fired when a tag is created
- `OnTagUpdated` - Fired when a tag is updated
- `OnTagDeleted` - Fired when a tag is deleted
- `OnTagsLoaded` - Fired when tags are loaded from storage
