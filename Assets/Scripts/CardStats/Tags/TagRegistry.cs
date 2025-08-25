using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// Central registry for managing all user-created tags in the TCG game.
/// Provides persistence, serialization, and efficient tag management.
/// </summary>
public class TagRegistry : MonoBehaviour
{
    [Header("Tag Registry Settings")]
    [SerializeField] private string saveFileName = "user_tags.json";
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveInterval = 30f; // seconds
    
    // Core collections
    private Dictionary<string, TagDefinition> _tagDefinitions = new Dictionary<string, TagDefinition>();
    private Dictionary<int, TagDefinition> _tagDefinitionsById = new Dictionary<int, TagDefinition>();
    private int _nextTagId = 1;
    
    // Events
    public static event Action<TagDefinition> OnTagCreated;
    public static event Action<TagDefinition> OnTagUpdated;
    public static event Action<TagDefinition> OnTagDeleted;
    public static event Action OnTagsLoaded;
    
    // Properties
    public IReadOnlyDictionary<string, TagDefinition> AllTags => _tagDefinitions;
    public int TagCount => _tagDefinitions.Count;
    
    // Singleton pattern
    public static TagRegistry Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTags();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (autoSave)
        {
            InvokeRepeating(nameof(SaveTags), autoSaveInterval, autoSaveInterval);
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && autoSave)
        {
            SaveTags();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && autoSave)
        {
            SaveTags();
        }
    }
    
    /// <summary>
    /// Creates a new tag definition and adds it to the registry
    /// </summary>
    public TagDefinition CreateTag(string name, string description = "", TagCategory category = TagCategory.Custom, bool isPermanent = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogError("Tag name cannot be null or empty");
            return null;
        }
        
        if (_tagDefinitions.ContainsKey(name))
        {
            Debug.LogWarning($"Tag '{name}' already exists");
            return null;
        }
        
        var tagDef = new TagDefinition
        {
            Id = _nextTagId++,
            Name = name.Trim(),
            Description = description?.Trim() ?? "",
            Category = category,
            IsPermanent = isPermanent,
            CreatedAt = DateTime.Now,
            LastModified = DateTime.Now
        };
        
        _tagDefinitions[tagDef.Name] = tagDef;
        _tagDefinitionsById[tagDef.Id] = tagDef;
        
        OnTagCreated?.Invoke(tagDef);
        
        if (autoSave)
        {
            SaveTags();
        }
        
        Debug.Log($"Created new tag: {tagDef.Name} (ID: {tagDef.Id})");
        return tagDef;
    }
    
    /// <summary>
    /// Updates an existing tag definition
    /// </summary>
    public bool UpdateTag(int tagId, string newName = null, string newDescription = null, TagCategory? newCategory = null, bool? newIsPermanent = null)
    {
        if (!_tagDefinitionsById.TryGetValue(tagId, out var tagDef))
        {
            Debug.LogError($"Tag with ID {tagId} not found");
            return false;
        }
        
        bool hasChanges = false;
        
        if (newName != null && newName != tagDef.Name)
        {
            if (_tagDefinitions.ContainsKey(newName))
            {
                Debug.LogError($"Tag name '{newName}' already exists");
                return false;
            }
            
            _tagDefinitions.Remove(tagDef.Name);
            tagDef.Name = newName.Trim();
            _tagDefinitions[tagDef.Name] = tagDef;
            hasChanges = true;
        }
        
        if (newDescription != null && newDescription != tagDef.Description)
        {
            tagDef.Description = newDescription.Trim();
            hasChanges = true;
        }
        
        if (newCategory.HasValue && newCategory.Value != tagDef.Category)
        {
            tagDef.Category = newCategory.Value;
            hasChanges = true;
        }
        
        if (newIsPermanent.HasValue && newIsPermanent.Value != tagDef.IsPermanent)
        {
            tagDef.IsPermanent = newIsPermanent.Value;
            hasChanges = true;
        }
        
        if (hasChanges)
        {
            tagDef.LastModified = DateTime.Now;
            OnTagUpdated?.Invoke(tagDef);
            
            if (autoSave)
            {
                SaveTags();
            }
            
            Debug.Log($"Updated tag: {tagDef.Name} (ID: {tagDef.Id})");
        }
        
        return hasChanges;
    }
    
    /// <summary>
    /// Deletes a tag definition from the registry
    /// </summary>
    public bool DeleteTag(int tagId)
    {
        if (!_tagDefinitionsById.TryGetValue(tagId, out var tagDef))
        {
            Debug.LogError($"Tag with ID {tagId} not found");
            return false;
        }
        
        _tagDefinitions.Remove(tagDef.Name);
        _tagDefinitionsById.Remove(tagId);
        
        OnTagDeleted?.Invoke(tagDef);
        
        if (autoSave)
        {
            SaveTags();
        }
        
        Debug.Log($"Deleted tag: {tagDef.Name} (ID: {tagDef.Id})");
        return true;
    }
    
    /// <summary>
    /// Gets a tag definition by name
    /// </summary>
    public TagDefinition GetTag(string name)
    {
        return _tagDefinitions.TryGetValue(name, out var tagDef) ? tagDef : null;
    }
    
    /// <summary>
    /// Gets a tag definition by ID
    /// </summary>
    public TagDefinition GetTagById(int id)
    {
        return _tagDefinitionsById.TryGetValue(id, out var tagDef) ? tagDef : null;
    }
    
    /// <summary>
    /// Checks if a tag exists by name
    /// </summary>
    public bool HasTag(string name)
    {
        return _tagDefinitions.ContainsKey(name);
    }
    
    /// <summary>
    /// Gets all tags in a specific category
    /// </summary>
    public IEnumerable<TagDefinition> GetTagsByCategory(TagCategory category)
    {
        return _tagDefinitions.Values.Where(t => t.Category == category);
    }
    
    /// <summary>
    /// Searches for tags by name (case-insensitive)
    /// </summary>
    public IEnumerable<TagDefinition> SearchTags(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return _tagDefinitions.Values;
            
        return _tagDefinitions.Values.Where(t => 
            t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Creates a Tag instance from a TagDefinition
    /// </summary>
    public Tag CreateTagInstance(TagDefinition tagDef)
    {
        if (tagDef == null) return null;
        
        // Create a concrete tag instance based on the definition
        return new UserCreatedTag(tagDef.Name, tagDef.IsPermanent, true);
    }
    
    /// <summary>
    /// Saves all tags to persistent storage
    /// </summary>
    public void SaveTags()
    {
        try
        {
            var saveData = new TagRegistrySaveData
            {
                NextTagId = _nextTagId,
                Tags = _tagDefinitions.Values.ToList()
            };
            
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
            
            File.WriteAllText(filePath, json);
            Debug.Log($"Saved {_tagDefinitions.Count} tags to {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save tags: {e.Message}");
        }
    }
    
    /// <summary>
    /// Loads all tags from persistent storage
    /// </summary>
    public void LoadTags()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
            
            if (!File.Exists(filePath))
            {
                Debug.Log("No saved tags found, starting with empty registry");
                return;
            }
            
            string json = File.ReadAllText(filePath);
            var saveData = JsonConvert.DeserializeObject<TagRegistrySaveData>(json);
            
            _tagDefinitions.Clear();
            _tagDefinitionsById.Clear();
            
            foreach (var tagDef in saveData.Tags)
            {
                _tagDefinitions[tagDef.Name] = tagDef;
                _tagDefinitionsById[tagDef.Id] = tagDef;
            }
            
            _nextTagId = saveData.NextTagId;
            
            Debug.Log($"Loaded {_tagDefinitions.Count} tags from {filePath}");
            OnTagsLoaded?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load tags: {e.Message}");
        }
    }
    
    /// <summary>
    /// Clears all tags from the registry
    /// </summary>
    public void ClearAllTags()
    {
        _tagDefinitions.Clear();
        _tagDefinitionsById.Clear();
        _nextTagId = 1;
        
        if (autoSave)
        {
            SaveTags();
        }
        
        Debug.Log("Cleared all tags from registry");
    }
    
    private void OnDestroy()
    {
        if (autoSave)
        {
            SaveTags();
        }
    }
}

/// <summary>
/// Represents a user-created tag definition
/// </summary>
[Serializable]
public class TagDefinition
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TagCategory Category { get; set; }
    public bool IsPermanent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
    
    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Category: {Category})";
    }
}

/// <summary>
/// Categories for organizing tags
/// </summary>
public enum TagCategory
{
    CardType,       // Creature, Spell, Equipment, etc.
    CardColor,      // Red, Blue, Green, etc.
    Subtype,        // Warrior, Mage, Beast, etc.
    Status,         // Frozen, Poisoned, Enraged, etc.
    Effect,         // Buffs, debuffs, conditions
    Custom          // User-defined categories
}

/// <summary>
/// Save data structure for the tag registry
/// </summary>
[Serializable]
public class TagRegistrySaveData
{
    public int NextTagId { get; set; }
    public List<TagDefinition> Tags { get; set; } = new List<TagDefinition>();
}

/// <summary>
/// Concrete implementation of a user-created tag
/// </summary>
public class UserCreatedTag : Tag
{
    public UserCreatedTag(string name, bool isPermanent, bool isActive) 
        : base(name, isPermanent, isActive)
    {
    }
    
    protected override void OnTagAdded()
    {
        // Handle tag-specific logic when added to a card
        Debug.Log($"User-created tag '{statName}' added to card");
    }
    
    protected override void OnTagRemoved()
    {
        // Handle tag-specific logic when removed from a card
        Debug.Log($"User-created tag '{statName}' removed from card");
    }
}
