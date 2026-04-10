using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Plugin.TagGenreManager.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.TagGenreManager.Api
{
    /// <summary>
    /// API controller for the Tag &amp; Genre Manager plugin.
    /// </summary>
    [ApiController]
    [Route("TagGenreManager")]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public class TagGenreController : ControllerBase
    {
        private readonly ILibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagGenreController"/> class.
        /// </summary>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
        public TagGenreController(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        // ---- TAGS ----

        /// <summary>Gets all defined tags, sorted alphabetically.</summary>
        /// <returns>Sorted list of defined tags.</returns>
        [HttpGet("Tags")]
        public ActionResult<List<ManagedItem>> GetTags()
        {
            return Plugin.Instance!.Configuration.Tags
                .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>Adds a new tag.</summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns>The added tag, or conflict if it already exists.</returns>
        [HttpPost("Tags")]
        public ActionResult AddTag([FromBody] ManagedItem tag)
        {
            var config = Plugin.Instance!.Configuration;
            if (config.Tags.Exists(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict("Tag already exists");
            }

            if (tag.Id == Guid.Empty)
            {
                tag.Id = Guid.NewGuid();
            }

            config.Tags.Add(tag);
            Plugin.Instance.SaveConfiguration();
            return Ok(tag);
        }

        /// <summary>Deletes a tag by id.</summary>
        /// <param name="id">The id of the tag to delete.</param>
        /// <returns>Ok if deleted, NotFound otherwise.</returns>
        [HttpDelete("Tags/{id}")]
        public ActionResult DeleteTag([FromRoute] Guid id)
        {
            var config = Plugin.Instance!.Configuration;
            var tag = config.Tags.FirstOrDefault(t => t.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            config.Tags.Remove(tag);
            Plugin.Instance.SaveConfiguration();
            return Ok();
        }

        // ---- GENRES ----

        /// <summary>Gets all defined genres, sorted alphabetically.</summary>
        /// <returns>Sorted list of defined genres.</returns>
        [HttpGet("Genres")]
        public ActionResult<List<ManagedItem>> GetGenres()
        {
            return Plugin.Instance!.Configuration.Genres
                .OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>Adds a new genre.</summary>
        /// <param name="genre">The genre to add.</param>
        /// <returns>The added genre, or conflict if it already exists.</returns>
        [HttpPost("Genres")]
        public ActionResult AddGenre([FromBody] ManagedItem genre)
        {
            var config = Plugin.Instance!.Configuration;
            if (config.Genres.Exists(g => g.Name.Equals(genre.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict("Genre already exists");
            }

            if (genre.Id == Guid.Empty)
            {
                genre.Id = Guid.NewGuid();
            }

            config.Genres.Add(genre);
            Plugin.Instance.SaveConfiguration();
            return Ok(genre);
        }

        /// <summary>Deletes a genre by id.</summary>
        /// <param name="id">The id of the genre to delete.</param>
        /// <returns>Ok if deleted, NotFound otherwise.</returns>
        [HttpDelete("Genres/{id}")]
        public ActionResult DeleteGenre([FromRoute] Guid id)
        {
            var config = Plugin.Instance!.Configuration;
            var genre = config.Genres.FirstOrDefault(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            config.Genres.Remove(genre);
            Plugin.Instance.SaveConfiguration();
            return Ok();
        }

        // ---- LIBRARIES ----

        /// <summary>Returns all top-level media libraries (virtual folders).</summary>
        /// <returns>A list of libraries with Id and Name.</returns>
        [HttpGet("Libraries")]
        public ActionResult<IEnumerable<object>> GetLibraries()
        {
            var folders = _libraryManager.GetUserRootFolder()
                .Children
                .OfType<Folder>()
                .Select(f => new { f.Id, f.Name })
                .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);

            return Ok(folders);
        }

        // ---- LIBRARY MEDIA DATABASE ----

        /// <summary>
        /// Returns ALL movies/series in the given library with their tags and genres.
        /// Used by the Library Browser tab to build a full media database view.
        /// </summary>
        /// <param name="libraryId">The Guid of the library folder to scan.</param>
        /// <returns>List of all media items in the library with tag/genre data.</returns>
        [HttpGet("LibraryMedia")]
        public ActionResult<IEnumerable<object>> GetLibraryMedia([FromQuery] Guid libraryId)
        {
            if (libraryId == Guid.Empty)
            {
                return BadRequest("libraryId is required.");
            }

            var query = new InternalItemsQuery
            {
                Recursive = true,
                IncludeItemTypes = new[]
                {
                    BaseItemKind.Movie,
                    BaseItemKind.Series
                },
                AncestorIds = new[] { libraryId },
                OrderBy = new[] { (ItemSortBy.SortName, SortOrder.Ascending) }
            };

            var allItems = _libraryManager.GetItemsResult(query).Items;

            var config = Plugin.Instance!.Configuration;
            var definedTagNames = config.Tags
                .Select(t => t.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var definedGenreNames = config.Genres
                .Select(g => g.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var result = allItems.Select(item => new
            {
                item.Id,
                item.Name,
                ProductionYear = item.ProductionYear,
                Type = item.GetType().Name,
                Tags = item.Tags ?? Array.Empty<string>(),
                Genres = item.Genres ?? Array.Empty<string>(),
                // Which of the item's tags/genres are plugin-managed vs custom/external
                ManagedTags = (item.Tags ?? Array.Empty<string>())
                    .Where(t => definedTagNames.Contains(t))
                    .ToArray(),
                ManagedGenres = (item.Genres ?? Array.Empty<string>())
                    .Where(g => definedGenreNames.Contains(g))
                    .ToArray()
            }).ToList();

            return Ok(result);
        }

        // ---- UNTAGGED MEDIA ----

        /// <summary>
        /// Returns all movies/series in the given library that are missing at least one
        /// plugin-defined tag or plugin-defined genre assignment.
        /// </summary>
        /// <param name="libraryId">The Guid of the library folder to scan.</param>
        /// <returns>List of media items that need tagging/genre assignment.</returns>
        [HttpGet("UntaggedMedia")]
        public ActionResult<IEnumerable<object>> GetUntaggedMedia([FromQuery] Guid libraryId)
        {
            if (libraryId == Guid.Empty)
            {
                return BadRequest("libraryId is required.");
            }

            var config = Plugin.Instance!.Configuration;

            var definedTagNames = config.Tags
                .Select(t => t.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var definedGenreNames = config.Genres
                .Select(g => g.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // AncestorIds is the correct way to scope a query to items inside a specific
            // library folder. TopParentIds has different semantics and was returning zero
            // results, which caused the frontend to show "all items are tagged".
            var query = new InternalItemsQuery
            {
                Recursive = true,
                IncludeItemTypes = new[]
                {
                    BaseItemKind.Movie,
                    BaseItemKind.Series
                },
                AncestorIds = new[] { libraryId },
                OrderBy = new[] { (ItemSortBy.SortName, SortOrder.Ascending) }
            };

            var allItems = _libraryManager.GetItemsResult(query).Items;

            var untagged = allItems
                .Where(item =>
                {
                    // If no tags are defined yet, every item needs attention.
                    bool hasDefinedTag = definedTagNames.Count > 0
                        && item.Tags != null
                        && item.Tags.Any(t => definedTagNames.Contains(t));

                    // If no genres are defined yet, treat genre as missing.
                    bool hasDefinedGenre = definedGenreNames.Count > 0
                        && item.Genres != null
                        && item.Genres.Any(g => definedGenreNames.Contains(g));

                    // Include item if it is missing a defined tag OR a defined genre.
                    return !hasDefinedTag || !hasDefinedGenre;
                })
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    ProductionYear = item.ProductionYear,
                    Type = item.GetType().Name,
                    Tags = item.Tags ?? Array.Empty<string>(),
                    Genres = item.Genres ?? Array.Empty<string>()
                })
                .ToList();

            return Ok(untagged);
        }
    }
}
