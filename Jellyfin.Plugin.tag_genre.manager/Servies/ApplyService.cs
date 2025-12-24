using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.TagGenreManager.Configuration;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;
using Jellyfin.Data.Enums;


namespace Jellyfin.Plugin.TagGenreManager.Services
{

public class ApplyService
{
    private readonly ILibraryManager _libraryManager;

    public ApplyService(ILibraryManager libraryManager)
    {
        _libraryManager = libraryManager;
    }

    public async Task ApplyToItem(Guid itemId, string[] tags, string[] genres)
    {
        var item = _libraryManager.GetItemById(itemId);
        if (item == null)
            return;

        // Apply metadata
        item.Tags = tags?.Distinct().ToArray() ?? Array.Empty<string>();
        item.Genres = genres?.Distinct().ToArray() ?? Array.Empty<string>();

        // Persist + reindex (THIS is the key call)
        await _libraryManager.UpdateItemAsync(
            item,
            item.GetParent(),
            ItemUpdateType.MetadataEdit,
            CancellationToken.None
        );
    }

    // Used by the search endpoint
    public IEnumerable<object> SearchItems(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Enumerable.Empty<object>();

        return _libraryManager
            .GetItemList(new InternalItemsQuery
            {
                SearchTerm = term,
                IncludeItemTypes = new[] { BaseItemKind.Series, BaseItemKind.Movie },
                Recursive = true
            })
            .Select(i => new
            {
                id = i.Id,
                name = i.Name,
                type = i.GetType().Name
            });
    }
}
}
