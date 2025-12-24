using System.Text.Json;
using Jellyfin.Plugin.CustomMeta;
using MediaBrowser.Common.Configuration;

namespace Jellyfin.Plugin.CustomMeta.Services;

public class PresetStore
{
    private readonly string _filePath;
    private PresetData _data;

    public PresetStore(IApplicationPaths paths)
    {
        _filePath = Path.Combine(paths.PluginConfigurationsPath, "CustomMeta.presets.json");
        Load();
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            _data = JsonSerializer.Deserialize<PresetData>(
                File.ReadAllText(_filePath)
            ) ?? new PresetData();
        }
        else
        {
            _data = new PresetData();
            Save();
        }
    }

    private void Save()
    {
        File.WriteAllText(
            _filePath,
            JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true })
        );
    }

    public string[] GetTags() => _data.Tags.OrderBy(x => x).ToArray();
    public string[] GetGenres() => _data.Genres.OrderBy(x => x).ToArray();

    public void AddTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (_data.Tags.Add(value.Trim()))
            Save();
    }

    public void AddGenre(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        if (_data.Genres.Add(value.Trim()))
            Save();
    }

    private class PresetData
    {
        public HashSet<string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Genres { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
