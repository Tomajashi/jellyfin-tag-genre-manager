
using System;
using System.Collections.Generic;
using System.Globalization;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Jellyfin.Plugin.TagGenreManager.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Jellyfin.Plugin.TagGenreManager.Controllers;
using Jellyfin.Plugin.TagGenreManager.Services;
using MediaBrowser.Controller.Plugins;


namespace Jellyfin.Plugin.TagGenreManager
{
    /// <summary>
    /// The main plugin.
    /// </summary>
    public class Plugin : BasePlugin, IHasWebPages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="serviceProvider">Instance of the <see cref="IServiceProvider"/> interface.</param>
        public Plugin(IServiceProvider serviceProvider)
        {
            Instance = this;
            ApplicationPaths = serviceProvider.GetRequiredService<IApplicationPaths>();
        }

        /// <inheritdoc />
        public override string Name => "TagGenreManager";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("4c1f8641-e980-4bcc-9fcc-29e221d2cf49");

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin? Instance { get; private set; }

        /// <summary>
        /// Gets the application paths.
        /// </summary>
        public IApplicationPaths ApplicationPaths { get; }

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "taggeneremanager",
                    EmbeddedResourcePath = "Jellyfin.Plugin.TagGenreManager.Configuration.configPage.html"
                }
            };
        }
    }
}
