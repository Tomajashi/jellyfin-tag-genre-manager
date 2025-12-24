using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Jellyfin.Plugin.TagGenreManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.TagGenreManager.Api
{
    [ApiController]
    [Route("TagGenreManager")]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize]
    public class TagGenreController : ControllerBase
    {
        [HttpGet("Tags")]
        public ActionResult<List<ManagedItem>> GetTags()
        {
            return Plugin.Instance!.Configuration.Tags;
        }

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

        [HttpGet("Genres")]
        public ActionResult<List<ManagedItem>> GetGenres()
        {
            return Plugin.Instance!.Configuration.Genres;
        }

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
    }
}
