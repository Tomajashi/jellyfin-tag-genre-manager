using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediaBrowser.Controller.Library;
using Jellyfin.Plugin.TagGenreManager.Services;
using Jellyfin.Plugin.TagGenreManager.Configuration;
using MediaBrowser.Controller.Plugins;

namespace Jellyfin.Plugin.TagGenreManager.Controllers;

[ApiController]
[Route("CustomMeta")]
public class CustomMetaController : ControllerBase
{
    private readonly ILibraryManager _libraryManager;
    private readonly PresetStore _presetStore;
    private readonly AssignmentStore _assignmentStore;
    private readonly ApplyService _applyService;

    public CustomMetaController(ILibraryManager libraryManager)
    {
        _libraryManager = libraryManager;
        var appPaths = Plugin.Instance.ApplicationPaths;
        _presetStore = new PresetStore(appPaths);
        _assignmentStore = new AssignmentStore(appPaths);
        _applyService = new ApplyService(libraryManager);
    }

    [HttpGet("presets")]
    public ActionResult<object> GetPresets()
        => Ok(new { tags = _presetStore.GetTags(), genres = _presetStore.GetGenres() });

    public record AddValueRequest(string Value);

    [HttpPost("presets/tags")]
    public IActionResult AddTag([FromBody] AddValueRequest req)
    {
        _presetStore.AddTag(req.Value);
        return NoContent();
    }

    [HttpPost("presets/genres")]
    public IActionResult AddGenre([FromBody] AddValueRequest req)
    {
        _presetStore.AddGenre(req.Value);
        return NoContent();
    }

    [HttpGet("search")]
    public ActionResult<object[]> Search([FromQuery] string term)
    {
        // Simple approach:
        // - query library for items matching name
        // - return shows/series (and optionally movies)
        // Implementation depends on your preferred query method.
        var results = _applyService.SearchItems(term);
        return Ok(results);
    }

    [HttpGet("item/{id}")]
    public ActionResult<object> GetItem(string id)
    {
        var guid = Guid.Parse(id);
        var item = _libraryManager.GetItemById(guid);
        if (item == null) return NotFound();

        var assignment = _assignmentStore.Get(guid);

        return Ok(new
        {
            id,
            name = item.Name,
            type = item.GetType().Name,
            tags = assignment.Tags,
            genres = assignment.Genres
        });
    }

    public record AssignmentDto(string[] Tags, string[] Genres);

    [HttpPost("item/{id}")]
    public async Task<IActionResult> SaveAndApply(string id, [FromBody] AssignmentDto dto)
    {
        var guid = Guid.Parse(id);
        _assignmentStore.Set(guid, dto.Tags, dto.Genres);

        await _applyService.ApplyToItem(guid, dto.Tags, dto.Genres);
        return NoContent();
    }
}
