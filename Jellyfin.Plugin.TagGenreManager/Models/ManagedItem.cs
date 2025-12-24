using System;

namespace Jellyfin.Plugin.TagGenreManager.Models;

/// <summary>
/// Represents a managed tag or genre item.
/// </summary>
public class ManagedItem
    {
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedItem"/> class.
    /// </summary>
    public ManagedItem()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the color (hex code, e.g., #FFFFFF).
    /// </summary>
    public string Color { get; set; } = string.Empty;
}
