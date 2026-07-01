namespace Spacesaver.Models;

public sealed class CleanupTask
{
    public required string Id { get; init; }
    public required int Number { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required TaskCategory Category { get; init; }
    public required RiskLevel RiskLevel { get; init; }
    public required string EstimatedSavings { get; init; }
    public required IReadOnlyList<string> Steps { get; init; }
    public string? Warning { get; init; }
    public bool RequiresAdmin { get; init; }
    public LaunchAction? PrimaryAction { get; init; }
    public string? CopyCommand { get; init; }
    public string IconGlyph { get; init; } = "\uE9F5";

    public string CategoryLabel => Category switch
    {
        TaskCategory.QuickWins => "Quick Wins",
        TaskCategory.SystemCleanup => "System Cleanup",
        TaskCategory.LargeSpaceWins => "Large Space Wins",
        TaskCategory.ManualReview => "Manual Review",
        _ => Category.ToString()
    };

    public string RiskLabel => RiskLevel switch
    {
        RiskLevel.Safe => "Safe",
        RiskLevel.Caution => "Caution",
        RiskLevel.Advanced => "Advanced",
        _ => RiskLevel.ToString()
    };
}
