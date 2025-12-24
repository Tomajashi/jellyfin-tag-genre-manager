using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.tag_genre.manager.Configuration;

public class CustomMetaConfigurationPage // : IPluginConfigurationPage
{
    public string Name => "Custom Metadata Manager";
    public string DisplayName => "Custom Metadata Manager";

    // public PluginPageInfo[] GetPages()
    //     => new[]
    //     {
    //         new PluginPageInfo
    //         {
    //             Name = Name,
    //             EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html"
    //         }
    //     };
}
