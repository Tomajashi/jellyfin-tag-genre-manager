using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Jellyfin.Plugin.CustomMeta.Configuration;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Common.Configuration;

namespace Jellyfin.Plugin.CustomMeta.Services;

public class AssignmentStore
{
    private readonly string _filePath;
    private Dictionary<Guid, Assignment> _assignments = new();

    public AssignmentStore(IApplicationPaths paths)
    {
        _filePath = Path.Combine(paths.PluginConfigurationsPath, "CustomMeta.assignments.json");
        Load();
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            _assignments = JsonSerializer.Deserialize<Dictionary<Guid, Assignment>>(
                File.ReadAllText(_filePath)
            ) ?? new();
        }
        else
        {
            _assignments = new();
            Save();
        }
    }

    private void Save()
    {
        File.WriteAllText(
            _filePath,
            JsonSerializer.Serialize(_assignments, new JsonSerializerOptions { WriteIndented = true })
        );
    }

    public Assignment Get(Guid itemId)
    {
        return _assignments.TryGetValue(itemId, out var a)
            ? a
            : new Assignment();
    }

    public void Set(Guid itemId, string[] tags, string[] genres)
    {
        _assignments[itemId] = new Assignment
        {
            Tags = tags?.Distinct().ToArray() ?? Array.Empty<string>(),
            Genres = genres?.Distinct().ToArray() ?? Array.Empty<string>()
        };
        Save();
    }

    public class Assignment
    {
        public string[] Tags { get; set; } = Array.Empty<string>();
        public string[] Genres { get; set; } = Array.Empty<string>();
    }
}
