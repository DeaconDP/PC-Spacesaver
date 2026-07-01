namespace Spacesaver.Models;

public enum ApplyOutcome
{
    Success,
    Skipped,
    Failed
}

public sealed class ApplyActionResult
{
    public required string TaskId { get; init; }
    public required string Title { get; init; }
    public required ApplyOutcome Outcome { get; init; }
    public required string Message { get; init; }
    public long BytesFreed { get; init; }
}
