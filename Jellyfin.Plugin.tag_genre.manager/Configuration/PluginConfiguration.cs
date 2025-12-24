using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.tag_genre.manager.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool Enabled { get; set; } = true;
}
