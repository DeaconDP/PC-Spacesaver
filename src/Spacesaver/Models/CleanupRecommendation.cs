namespace Spacesaver.Models;

public sealed class CleanupRecommendation
{
    public required string TaskId { get; init; }
    public required CleanupTask Task { get; init; }
    public long ReclaimableBytes { get; init; }
    public required ImpactLevel Impact { get; init; }
    public required ExecutionMode ExecutionMode { get; init; }
    public required string ScanDetail { get; init; }
    public required string RiskSummary { get; init; }
    public bool IsApplicable { get; init; } = true;
    public bool IsSelected { get; set; }

    public static string BuildRiskSummary(CleanupTask task)
    {
        var parts = new List<string> { $"Risk: {task.RiskLabel}." };

        if (task.RequiresAdmin)
            parts.Add("Requires administrator privileges.");

        if (!string.IsNullOrWhiteSpace(task.Warning))
            parts.Add(task.Warning);
        else if (task.Steps.Count > 0)
            parts.Add(task.Steps[0]);

        return string.Join(" ", parts);
    }

    public static bool DefaultSelected(CleanupTask task, ExecutionMode mode, long bytes)
    {
        if (task.RiskLevel is RiskLevel.Caution or RiskLevel.Advanced)
            return false;

        if (mode is ExecutionMode.ManualOnly)
            return false;

        return bytes > 0 || task.RiskLevel == RiskLevel.Safe;
    }
}
