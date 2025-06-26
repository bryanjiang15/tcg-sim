using System.Collections.Generic;
using UnityEngine;

/**
 * Interface for objects that can have tags attached to them.
 * Provides methods for managing tags on an object.
 */
public interface ITaggable
{
    /// <summary>
    /// Collection of all tags currently attached to this object
    /// </summary>
    IReadOnlyCollection<Tag> Tags { get; }

    /// <summary>
    /// Attempts to add a tag to this object
    /// </summary>
    /// <param name="tag">The tag to add</param>
    /// <returns>True if the tag was successfully added</returns>
    bool AddTag(Tag tag);

    /// <summary>
    /// Attempts to remove a tag from this object
    /// </summary>
    /// <param name="tag">The tag to remove</param>
    /// <returns>True if the tag was successfully removed</returns>
    bool RemoveTag(Tag tag);

    /// <summary>
    /// Checks if this object has a tag with the specified name
    /// </summary>
    /// <param name="tagName">The name of the tag to check for</param>
    /// <returns>True if the object has the tag</returns>
    bool HasTag(string tagName);

    /// <summary>
    /// Gets a tag by its name
    /// </summary>
    /// <param name="tagName">The name of the tag to get</param>
    /// <returns>The tag if found, null otherwise</returns>
    Tag GetTag(string tagName);

    /// <summary>
    /// Gets a tag by its ID
    /// </summary>
    /// <param name="tagId">The ID of the tag to get</param>
    /// <returns>The tag if found, null otherwise</returns>
    Tag GetTagById(int tagId);
}
