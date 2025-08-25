using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Helper class for managing tags and integrating with the TagRegistry system.
/// Provides convenient methods for working with user-created tags.
/// </summary>
public static class TagManager
{
    /// <summary>
    /// Gets the TagRegistry instance
    /// </summary>
    public static TagRegistry Registry => TagRegistry.Instance;
    
    /// <summary>
    /// Creates a new tag with the specified parameters
    /// </summary>
    public static TagDefinition CreateTag(string name, string description = "", TagCategory category = TagCategory.Custom, bool isPermanent = false)
    {
        if (Registry == null)
        {
            Debug.LogError("TagRegistry not found. Make sure TagRegistry is in the scene.");
            return null;
        }
        
        return Registry.CreateTag(name, description, category, isPermanent);
    }
    
    /// <summary>
    /// Gets a tag definition by name
    /// </summary>
    public static TagDefinition GetTag(string name)
    {
        return Registry?.GetTag(name);
    }
    
    /// <summary>
    /// Gets a tag definition by ID
    /// </summary>
    public static TagDefinition GetTagById(int id)
    {
        return Registry?.GetTagById(id);
    }
    
    /// <summary>
    /// Checks if a tag exists by name
    /// </summary>
    public static bool HasTag(string name)
    {
        return Registry?.HasTag(name) ?? false;
    }
    
    /// <summary>
    /// Creates a Tag instance from a tag name
    /// </summary>
    public static Tag CreateTagInstance(string tagName)
    {
        var tagDef = GetTag(tagName);
        return tagDef != null ? Registry.CreateTagInstance(tagDef) : null;
    }
    
    /// <summary>
    /// Creates a Tag instance from a tag ID
    /// </summary>
    public static Tag CreateTagInstance(int tagId)
    {
        var tagDef = GetTagById(tagId);
        return tagDef != null ? Registry.CreateTagInstance(tagDef) : null;
    }
    
    /// <summary>
    /// Adds a tag to a taggable object by name
    /// </summary>
    public static bool AddTagToObject(ITaggable taggable, string tagName)
    {
        if (taggable == null || string.IsNullOrWhiteSpace(tagName))
            return false;
            
        var tag = CreateTagInstance(tagName);
        return tag != null && taggable.AddTag(tag);
    }
    
    /// <summary>
    /// Adds a tag to a taggable object by ID
    /// </summary>
    public static bool AddTagToObject(ITaggable taggable, int tagId)
    {
        if (taggable == null)
            return false;
            
        var tag = CreateTagInstance(tagId);
        return tag != null && taggable.AddTag(tag);
    }
    
    /// <summary>
    /// Removes a tag from a taggable object by name
    /// </summary>
    public static bool RemoveTagFromObject(ITaggable taggable, string tagName)
    {
        if (taggable == null || string.IsNullOrWhiteSpace(tagName))
            return false;
            
        var tag = taggable.GetTag(tagName);
        return tag != null && taggable.RemoveTag(tag);
    }
    
    /// <summary>
    /// Gets all tags in a specific category
    /// </summary>
    public static IEnumerable<TagDefinition> GetTagsByCategory(TagCategory category)
    {
        return Registry?.GetTagsByCategory(category) ?? Enumerable.Empty<TagDefinition>();
    }
    
    /// <summary>
    /// Searches for tags by name or description
    /// </summary>
    public static IEnumerable<TagDefinition> SearchTags(string searchTerm)
    {
        return Registry?.SearchTags(searchTerm) ?? Enumerable.Empty<TagDefinition>();
    }
    
    /// <summary>
    /// Gets all available tag categories
    /// </summary>
    public static IEnumerable<TagCategory> GetAllCategories()
    {
        return System.Enum.GetValues(typeof(TagCategory)).Cast<TagCategory>();
    }
    
    /// <summary>
    /// Gets the display name for a tag category
    /// </summary>
    public static string GetCategoryDisplayName(TagCategory category)
    {
        return category switch
        {
            TagCategory.CardType => "Card Type",
            TagCategory.CardColor => "Card Color",
            TagCategory.Subtype => "Subtype",
            TagCategory.Status => "Status",
            TagCategory.Effect => "Effect",
            TagCategory.Custom => "Custom",
            _ => category.ToString()
        };
    }
    
    /// <summary>
    /// Gets all tags that can be applied to a specific object
    /// </summary>
    public static IEnumerable<TagDefinition> GetAvailableTagsForObject(ITaggable taggable)
    {
        if (Registry == null || taggable == null)
            return Enumerable.Empty<TagDefinition>();
            
        var existingTagNames = taggable.Tags.Select(t => t.statName).ToHashSet();
        return Registry.AllTags.Values.Where(t => !existingTagNames.Contains(t.Name));
    }
    
    /// <summary>
    /// Gets all tags currently applied to an object
    /// </summary>
    public static IEnumerable<TagDefinition> GetAppliedTagsForObject(ITaggable taggable)
    {
        if (Registry == null || taggable == null)
            return Enumerable.Empty<TagDefinition>();
            
        return taggable.Tags
            .Select(t => Registry.GetTag(t.statName))
            .Where(t => t != null);
    }
    
    /// <summary>
    /// Validates a tag name (checks for invalid characters, length, etc.)
    /// </summary>
    public static bool IsValidTagName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
            
        // Check length
        if (name.Length > 50)
            return false;
            
        // Check for invalid characters
        if (name.Any(c => char.IsControl(c) || c == '/' || c == '\\' || c == ':' || c == '*' || c == '?' || c == '"' || c == '<' || c == '>' || c == '|'))
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Gets a suggested tag name (removes invalid characters, trims, etc.)
    /// </summary>
    public static string GetSuggestedTagName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";
            
        // Remove invalid characters
        var validChars = input.Where(c => !char.IsControl(c) && c != '/' && c != '\\' && c != ':' && c != '*' && c != '?' && c != '"' && c != '<' && c != '>' && c != '|').ToArray();
        
        var result = new string(validChars).Trim();
        
        // Limit length
        if (result.Length > 50)
            result = result.Substring(0, 50);
            
        return result;
    }
    
    /// <summary>
    /// Gets statistics about tag usage
    /// </summary>
    public static TagStatistics GetTagStatistics()
    {
        if (Registry == null)
            return new TagStatistics();
            
        var tags = Registry.AllTags.Values.ToList();
        
        return new TagStatistics
        {
            TotalTags = tags.Count,
            TagsByCategory = tags.GroupBy(t => t.Category).ToDictionary(g => g.Key, g => g.Count()),
            PermanentTags = tags.Count(t => t.IsPermanent),
            TemporaryTags = tags.Count(t => !t.IsPermanent),
            MostRecentTag = tags.OrderByDescending(t => t.LastModified).FirstOrDefault()
        };
    }
}

/// <summary>
/// Statistics about tag usage
/// </summary>
public class TagStatistics
{
    public int TotalTags { get; set; }
    public Dictionary<TagCategory, int> TagsByCategory { get; set; } = new Dictionary<TagCategory, int>();
    public int PermanentTags { get; set; }
    public int TemporaryTags { get; set; }
    public TagDefinition MostRecentTag { get; set; }
}
