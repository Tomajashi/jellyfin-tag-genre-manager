using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TagGenreManager.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool Enabled { get; set; } = true;
}
