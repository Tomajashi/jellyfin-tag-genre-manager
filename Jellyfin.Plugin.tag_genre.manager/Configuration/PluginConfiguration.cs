using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CustomMeta.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool Enabled { get; set; } = true;
}
