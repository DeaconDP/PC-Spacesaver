namespace Spacesaver.Models;

public sealed class DiskScanResult
{
    public required string DriveName { get; init; }
    public long TotalBytes { get; init; }
    public long FreeBytes { get; init; }
    public double UsedPercent { get; init; }
    public required IReadOnlyList<CleanupRecommendation> Recommendations { get; init; }
}
