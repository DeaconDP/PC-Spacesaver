using System.Runtime.InteropServices;
using Spacesaver.Models;

namespace Spacesaver.Services;

public sealed class CleanupExecutionService
{
    private static readonly TimeSpan TempFileMinAge = TimeSpan.FromDays(3);

    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? rootPath, RecycleFlags flags);

    [Flags]
    private enum RecycleFlags : uint
    {
        SHERB_NOCONFIRMATION = 0x00000001,
        SHERB_NOPROGRESSUI = 0x00000002,
        SHERB_NOSOUND = 0x00000004
    }

    public async Task<IReadOnlyList<ApplyActionResult>> ApplyAsync(
        IEnumerable<CleanupRecommendation> recommendations,
        IProgress<(string TaskId, string Status)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ApplyActionResult>();

        foreach (var recommendation in recommendations.Where(r => r.IsSelected))
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report((recommendation.TaskId, $"Applying {recommendation.Task.Title}…"));

            var result = recommendation.ExecutionMode switch
            {
                ExecutionMode.AutoClean => await ExecuteAutoCleanAsync(recommendation),
                ExecutionMode.LaunchTool => await ExecuteLaunchAsync(recommendation),
                ExecutionMode.ManualOnly => await ExecuteManualAsync(recommendation),
                _ => CreateResult(recommendation, ApplyOutcome.Skipped, "Unknown execution mode.", 0)
            };

            results.Add(result);

            if (result.Outcome == ApplyOutcome.Success && recommendation.ExecutionMode == ExecutionMode.AutoClean)
                AppServices.Progress.SetCompleted(recommendation.TaskId, true);
        }

        return results;
    }

    private static async Task<ApplyActionResult> ExecuteAutoCleanAsync(CleanupRecommendation recommendation)
    {
        if (recommendation.Task.RiskLevel is RiskLevel.Advanced or RiskLevel.Caution
            || !string.IsNullOrWhiteSpace(recommendation.Task.Warning))
        {
            return CreateResult(
                recommendation,
                ApplyOutcome.Skipped,
                "This action requires manual review — opening the recommended tool instead.",
                0);
        }

        return recommendation.TaskId.ToLowerInvariant() switch
        {
            "recycle-bin" => await Task.Run(EmptyRecycleBin),
            "user-temp" => await Task.Run(() => CleanUserTempFolder(recommendation.ReclaimableBytes)),
            _ => await ExecuteLaunchAsync(recommendation)
        };
    }

    private static ApplyActionResult EmptyRecycleBin()
    {
        try
        {
            var hr = SHEmptyRecycleBin(IntPtr.Zero, null,
                RecycleFlags.SHERB_NOCONFIRMATION | RecycleFlags.SHERB_NOPROGRESSUI | RecycleFlags.SHERB_NOSOUND);

            return hr == 0
                ? new ApplyActionResult
                {
                    TaskId = "recycle-bin",
                    Title = "Empty Recycle Bin",
                    Outcome = ApplyOutcome.Success,
                    Message = "Recycle Bin emptied.",
                    BytesFreed = 0
                }
                : new ApplyActionResult
                {
                    TaskId = "recycle-bin",
                    Title = "Empty Recycle Bin",
                    Outcome = ApplyOutcome.Failed,
                    Message = $"Could not empty Recycle Bin (error {hr}).",
                    BytesFreed = 0
                };
        }
        catch (Exception ex)
        {
            return new ApplyActionResult
            {
                TaskId = "recycle-bin",
                Title = "Empty Recycle Bin",
                Outcome = ApplyOutcome.Failed,
                Message = ex.Message,
                BytesFreed = 0
            };
        }
    }

    private static ApplyActionResult CleanUserTempFolder(long estimatedBytes)
    {
        var tempPath = Environment.GetEnvironmentVariable("TEMP");
        if (string.IsNullOrWhiteSpace(tempPath) || !Directory.Exists(tempPath))
        {
            return new ApplyActionResult
            {
                TaskId = "user-temp",
                Title = "Clear User Temp Folder",
                Outcome = ApplyOutcome.Skipped,
                Message = "Temp folder not found.",
                BytesFreed = 0
            };
        }

        var cutoff = DateTime.UtcNow - TempFileMinAge;
        var deleted = 0;
        var skipped = 0;
        long bytesFreed = 0;

        try
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(tempPath))
            {
                try
                {
                    var info = new FileInfo(entry);
                    if (!info.Exists)
                    {
                        var dirInfo = new DirectoryInfo(entry);
                        if (!dirInfo.Exists)
                            continue;

                        if (dirInfo.LastWriteTimeUtc > cutoff)
                        {
                            skipped++;
                            continue;
                        }

                        var dirSize = MeasureEntrySize(dirInfo);
                        dirInfo.Delete(true);
                        deleted++;
                        bytesFreed += dirSize;
                        continue;
                    }

                    if (info.LastWriteTimeUtc > cutoff)
                    {
                        skipped++;
                        continue;
                    }

                    var size = info.Length;
                    info.Delete();
                    deleted++;
                    bytesFreed += size;
                }
                catch (IOException)
                {
                    skipped++;
                }
                catch (UnauthorizedAccessException)
                {
                    skipped++;
                }
            }
        }
        catch (Exception ex)
        {
            return new ApplyActionResult
            {
                TaskId = "user-temp",
                Title = "Clear User Temp Folder",
                Outcome = ApplyOutcome.Failed,
                Message = ex.Message,
                BytesFreed = bytesFreed
            };
        }

        var message = $"Removed {deleted} item(s)";
        if (skipped > 0)
            message += $", skipped {skipped} in-use or recent file(s)";
        message += ".";

        return new ApplyActionResult
        {
            TaskId = "user-temp",
            Title = "Clear User Temp Folder",
            Outcome = deleted > 0 ? ApplyOutcome.Success : ApplyOutcome.Skipped,
            Message = message,
            BytesFreed = bytesFreed > 0 ? bytesFreed : Math.Min(estimatedBytes, bytesFreed)
        };
    }

    private static long MeasureEntrySize(DirectoryInfo directory)
    {
        long total = 0;
        try
        {
            foreach (var file in directory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try { total += file.Length; }
                catch { /* skip */ }
            }
        }
        catch { /* skip */ }

        return total;
    }

    private static async Task<ApplyActionResult> ExecuteLaunchAsync(CleanupRecommendation recommendation)
    {
        if (recommendation.Task.PrimaryAction is null)
        {
            return CreateResult(
                recommendation,
                ApplyOutcome.Skipped,
                "No tool available — see task details for manual steps.",
                0);
        }

        var launched = await AppServices.Launcher.LaunchAsync(recommendation.Task.PrimaryAction);
        return CreateResult(
            recommendation,
            launched ? ApplyOutcome.Success : ApplyOutcome.Failed,
            launched
                ? "Opened — complete the steps in the Windows tool."
                : "Could not open the tool. Try opening it manually from Start or Settings.",
            0);
    }

    private static Task<ApplyActionResult> ExecuteManualAsync(CleanupRecommendation recommendation)
    {
        if (recommendation.Task.PrimaryAction is not null)
            return ExecuteLaunchAsync(recommendation);

        return Task.FromResult(CreateResult(
            recommendation,
            ApplyOutcome.Skipped,
            "Manual steps required — open task details for guidance.",
            0));
    }

    private static ApplyActionResult CreateResult(
        CleanupRecommendation recommendation,
        ApplyOutcome outcome,
        string message,
        long bytesFreed) =>
        new()
        {
            TaskId = recommendation.TaskId,
            Title = recommendation.Task.Title,
            Outcome = outcome,
            Message = message,
            BytesFreed = bytesFreed
        };
}
