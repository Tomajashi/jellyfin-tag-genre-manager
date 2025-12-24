using System.Collections.Generic;
using Jellyfin.Plugin.TagGenreManager.Models;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TagGenreManager.Configuration;

/// <summary>
/// Plugin configuration for Tag and Genre Manager.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
    {
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Tags = new List<ManagedItem>();
        Genres = new List<ManagedItem>();
    }

    /// <summary>
    /// Gets or sets the list of custom tags.
    /// </summary>
    public List<ManagedItem> Tags { get; set; }

    /// <summary>
    /// Gets or sets the list of custom genres.
    /// </summary>
    public List<ManagedItem> Genres { get; set; }
}
