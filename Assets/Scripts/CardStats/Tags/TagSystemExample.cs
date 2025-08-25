using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Example script demonstrating how to use the TagRegistry and TagManager systems.
/// This shows various ways to create, manage, and apply user-created tags.
/// </summary>
public class TagSystemExample : MonoBehaviour
{
    [Header("Example Settings")]
    [SerializeField] private bool createExampleTagsOnStart = true;
    [SerializeField] private bool demonstrateTagUsage = true;
    
    // Example card that implements ITaggable
    private ExampleCard exampleCard;
    
    private void Start()
    {
        // Wait for TagRegistry to initialize
        StartCoroutine(InitializeTagSystem());
    }
    
    private System.Collections.IEnumerator InitializeTagSystem()
    {
        // Wait for TagRegistry to be available
        while (TagRegistry.Instance == null)
        {
            yield return null;
        }
        
        // Subscribe to tag events
        TagRegistry.OnTagCreated += OnTagCreated;
        TagRegistry.OnTagUpdated += OnTagUpdated;
        TagRegistry.OnTagDeleted += OnTagDeleted;
        TagRegistry.OnTagsLoaded += OnTagsLoaded;
        
        if (createExampleTagsOnStart)
        {
            CreateExampleTags();
        }
        
        if (demonstrateTagUsage)
        {
            DemonstrateTagUsage();
        }
        
        // Display tag statistics
        DisplayTagStatistics();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (TagRegistry.Instance != null)
        {
            TagRegistry.OnTagCreated -= OnTagCreated;
            TagRegistry.OnTagUpdated -= OnTagUpdated;
            TagRegistry.OnTagDeleted -= OnTagDeleted;
            TagRegistry.OnTagsLoaded -= OnTagsLoaded;
        }
    }
    
    /// <summary>
    /// Creates example tags to demonstrate the system
    /// </summary>
    private void CreateExampleTags()
    {
        Debug.Log("=== Creating Example Tags ===");
        
        // Create card type tags
        TagManager.CreateTag("Dragon", "Powerful flying creatures", TagCategory.CardType, true);
        TagManager.CreateTag("Warrior", "Skilled fighters", TagCategory.Subtype, true);
        TagManager.CreateTag("Spell", "Magical abilities", TagCategory.CardType, true);
        
        // Create status effect tags
        TagManager.CreateTag("Frozen", "Cannot attack or defend", TagCategory.Status, false);
        TagManager.CreateTag("Poisoned", "Takes damage over time", TagCategory.Status, false);
        TagManager.CreateTag("Enraged", "Increased attack power", TagCategory.Effect, false);
        
        // Create custom tags
        TagManager.CreateTag("Rare", "Hard to find cards", TagCategory.Custom, true);
        TagManager.CreateTag("Limited Edition", "Special promotional cards", TagCategory.Custom, true);
        TagManager.CreateTag("Foil", "Shiny variant cards", TagCategory.Custom, true);
        
        Debug.Log("Example tags created successfully!");
    }
    
    /// <summary>
    /// Demonstrates how to use tags with taggable objects
    /// </summary>
    private void DemonstrateTagUsage()
    {
        Debug.Log("=== Demonstrating Tag Usage ===");
        
        // Create an example card
        exampleCard = new ExampleCard("Dragon Warrior");
        
        // Add some tags to the card
        TagManager.AddTagToObject(exampleCard, "Dragon");
        TagManager.AddTagToObject(exampleCard, "Warrior");
        TagManager.AddTagToObject(exampleCard, "Rare");
        
        // Display the card's tags
        Debug.Log($"Card '{exampleCard.Name}' has {exampleCard.Tags.Count} tags:");
        foreach (var tag in exampleCard.Tags)
        {
            Debug.Log($"  - {tag.statName}");
        }
        
        // Check if card has specific tags
        Debug.Log($"Is Dragon: {exampleCard.HasTag("Dragon")}");
        Debug.Log($"Is Spell: {exampleCard.HasTag("Spell")}");
        
        // Add a temporary status effect
        TagManager.AddTagToObject(exampleCard, "Enraged");
        Debug.Log($"Added Enraged status. Total tags: {exampleCard.Tags.Count}");
        
        // Remove a tag
        TagManager.RemoveTagFromObject(exampleCard, "Enraged");
        Debug.Log($"Removed Enraged status. Total tags: {exampleCard.Tags.Count}");
        
        // Search for available tags
        var availableTags = TagManager.GetAvailableTagsForObject(exampleCard);
        Debug.Log($"Available tags for this card: {string.Join(", ", availableTags.Select(t => t.Name))}");
    }
    
    /// <summary>
    /// Displays statistics about the tag system
    /// </summary>
    private void DisplayTagStatistics()
    {
        var stats = TagManager.GetTagStatistics();
        
        Debug.Log("=== Tag Statistics ===");
        Debug.Log($"Total Tags: {stats.TotalTags}");
        Debug.Log($"Permanent Tags: {stats.PermanentTags}");
        Debug.Log($"Temporary Tags: {stats.TemporaryTags}");
        
        Debug.Log("Tags by Category:");
        foreach (var category in stats.TagsByCategory)
        {
            Debug.Log($"  {TagManager.GetCategoryDisplayName(category.Key)}: {category.Value}");
        }
        
        if (stats.MostRecentTag != null)
        {
            Debug.Log($"Most Recent Tag: {stats.MostRecentTag.Name} (Modified: {stats.MostRecentTag.LastModified})");
        }
    }
    
    /// <summary>
    /// Demonstrates tag search functionality
    /// </summary>
    [ContextMenu("Search Tags")]
    public void SearchTagsExample()
    {
        Debug.Log("=== Tag Search Examples ===");
        
        // Search for tags containing "dragon"
        var dragonTags = TagManager.SearchTags("dragon");
        Debug.Log($"Tags containing 'dragon': {string.Join(", ", dragonTags.Select(t => t.Name))}");
        
        // Search for tags containing "effect"
        var effectTags = TagManager.SearchTags("effect");
        Debug.Log($"Tags containing 'effect': {string.Join(", ", effectTags.Select(t => t.Name))}");
        
        // Get all tags in a specific category
        var statusTags = TagManager.GetTagsByCategory(TagCategory.Status);
        Debug.Log($"Status tags: {string.Join(", ", statusTags.Select(t => t.Name))}");
    }
    
    /// <summary>
    /// Demonstrates tag management operations
    /// </summary>
    [ContextMenu("Manage Tags")]
    public void ManageTagsExample()
    {
        Debug.Log("=== Tag Management Examples ===");
        
        // Create a new tag
        var newTag = TagManager.CreateTag("Legendary", "Ultra rare and powerful cards", TagCategory.Custom, true);
        if (newTag != null)
        {
            Debug.Log($"Created new tag: {newTag.Name} (ID: {newTag.Id})");
            
            // Update the tag
            TagManager.Registry.UpdateTag(newTag.Id, description: "Updated description for legendary cards");
            Debug.Log($"Updated tag description for: {newTag.Name}");
            
            // Delete the tag
            TagManager.Registry.DeleteTag(newTag.Id);
            Debug.Log($"Deleted tag: {newTag.Name}");
        }
    }
    
    /// <summary>
    /// Demonstrates tag validation
    /// </summary>
    [ContextMenu("Validate Tags")]
    public void ValidateTagsExample()
    {
        Debug.Log("=== Tag Validation Examples ===");
        
        string[] testNames = { "Valid Tag", "", "Invalid/Tag", "Very Long Tag Name That Exceeds The Maximum Length Limit", "Normal Tag" };
        
        foreach (var name in testNames)
        {
            bool isValid = TagManager.IsValidTagName(name);
            string suggested = TagManager.GetSuggestedTagName(name);
            
            Debug.Log($"Name: '{name}' -> Valid: {isValid}, Suggested: '{suggested}'");
        }
    }
    
    // Event handlers
    private void OnTagCreated(TagDefinition tagDef)
    {
        Debug.Log($"Tag created: {tagDef.Name} (Category: {tagDef.Category})");
    }
    
    private void OnTagUpdated(TagDefinition tagDef)
    {
        Debug.Log($"Tag updated: {tagDef.Name}");
    }
    
    private void OnTagDeleted(TagDefinition tagDef)
    {
        Debug.Log($"Tag deleted: {tagDef.Name}");
    }
    
    private void OnTagsLoaded()
    {
        Debug.Log("Tags loaded from persistent storage");
    }
}

/// <summary>
/// Example implementation of a taggable card
/// </summary>
public class ExampleCard : ITaggable
{
    public string Name { get; private set; }
    private List<Tag> tags = new List<Tag>();
    
    public ExampleCard(string name)
    {
        Name = name;
    }
    
    public IReadOnlyCollection<Tag> Tags => tags.AsReadOnly();
    
    public bool AddTag(Tag tag)
    {
        if (tag == null || HasTag(tag.statName))
            return false;
            
        // Check if tag is permanent and already exists
        if (tag.isPermanent && tags.Any(t => t.statName == tag.statName))
            return false;
            
        tags.Add(tag);
        tag.OnTagAdded();
        return true;
    }
    
    public bool RemoveTag(Tag tag)
    {
        if (tag == null || !tags.Contains(tag))
            return false;
            
        // Check if tag is permanent
        if (tag.isPermanent)
        {
            Debug.LogWarning($"Cannot remove permanent tag: {tag.statName}");
            return false;
        }
        
        tags.Remove(tag);
        tag.OnTagRemoved();
        return true;
    }
    
    public bool HasTag(string tagName)
    {
        return tags.Any(t => t.statName == tagName);
    }
    
    public Tag GetTag(string tagName)
    {
        return tags.FirstOrDefault(t => t.statName == tagName);
    }
    
    public Tag GetTagById(int tagId)
    {
        // This would need to be implemented based on your tag ID system
        // For now, we'll return null
        return null;
    }
}
