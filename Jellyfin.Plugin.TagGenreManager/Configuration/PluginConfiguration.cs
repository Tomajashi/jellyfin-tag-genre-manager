using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        Tags = new Collection<ManagedItem>();
        Genres = new Collection<ManagedItem>();
    }

    /// <summary>
    /// Gets the list of custom tags.
    /// </summary>
    public Collection<ManagedItem> Tags { get; }

    /// <summary>
    /// Gets the list of custom genres.
    /// </summary>
    public Collection<ManagedItem> Genres { get; }
}
