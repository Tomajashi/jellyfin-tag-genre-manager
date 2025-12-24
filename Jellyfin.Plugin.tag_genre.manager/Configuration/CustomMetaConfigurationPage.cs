using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CustomMeta.Configuration;

public class CustomMetaConfigurationPage : IPluginConfigurationPage
{
    public string Name => "Custom Metadata Manager";
    public string DisplayName => "Custom Metadata Manager";

    public PluginPageInfo[] GetPages()
        => new[]
        {
            new PluginPageInfo
            {
                Name = "custommeta",
                EmbeddedResourcePath = $"{GetType().Namespace}.configPage.html"
            }
        };
}
