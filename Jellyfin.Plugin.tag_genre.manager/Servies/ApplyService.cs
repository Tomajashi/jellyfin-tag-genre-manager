using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;

namespace Jellyfin.Plugin.CustomMeta.Services;

public class ApplyService
{
    private readonly ILibraryManager _libraryManager;

    public ApplyService(ILibraryManager libraryManager)
    {
        _libraryManager = libraryManager;
    }

    public void ApplyToItem(Guid itemId, string[] tags, string[] genres)
    {
        var item = _libraryManager.GetItemById(itemId);
        if (item == null)
            return;

        // Apply metadata
        item.Tags = tags?.Distinct().ToArray() ?? Array.Empty<string>();
        item.Genres = genres?.Distinct().ToArray() ?? Array.Empty<string>();

        // Persist + reindex (THIS is the key call)
        _libraryManager.UpdateItem(
            item,
            item.GetParent(),
            ItemUpdateType.MetadataEdit
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
                IncludeItemTypes = new[] { "Series", "Movie" },
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
