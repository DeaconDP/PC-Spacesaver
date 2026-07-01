using System.Text.Json;

namespace Spacesaver.Services;

public sealed class ProgressService
{
    private const int TaskCount = 20;
    private static readonly string StoragePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PCSpacesaver",
        "progress.json");

    private readonly HashSet<string> _completed = new(StringComparer.OrdinalIgnoreCase);

    public event Action? ProgressChanged;

    public int TotalTasks => TaskCount;
    public int CompletedCount => _completed.Count;

    public ProgressService()
    {
        Load();
    }

    public bool IsCompleted(string taskId) => _completed.Contains(taskId);

    public void SetCompleted(string taskId, bool completed)
    {
        if (completed)
            _completed.Add(taskId);
        else
            _completed.Remove(taskId);

        Save();
        ProgressChanged?.Invoke();
    }

    public void Reset()
    {
        _completed.Clear();
        Save();
        ProgressChanged?.Invoke();
    }

    private void Load()
    {
        if (!File.Exists(StoragePath))
            return;

        try
        {
            var json = File.ReadAllText(StoragePath);
            var ids = JsonSerializer.Deserialize<List<string>>(json);
            if (ids is null)
                return;

            foreach (var id in ids)
                _completed.Add(id);
        }
        catch
        {
            _completed.Clear();
        }
    }

    private void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(StoragePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(_completed.ToList());
            File.WriteAllText(StoragePath, json);
        }
        catch
        {
            // Ignore persistence failures; progress is session-only.
        }
    }
}
