using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Spacesaver.Helpers;
using Spacesaver.Models;

namespace Spacesaver.ViewModels;

public sealed class TaskRowViewModel
{
    public TaskRowViewModel(CleanupTask task, bool isCompleted)
    {
        Task = task;
        IsCompleted = isCompleted;
    }

    public CleanupTask Task { get; }
    public bool IsCompleted { get; set; }
    public string Subtitle => $"#{Task.Number} · {Task.EstimatedSavings} · {Task.RiskLabel}";
    public Visibility DoneIconVisibility => IsCompleted ? Visibility.Visible : Visibility.Collapsed;
    public Brush RiskBrush => RiskLevelHelper.GetBadgeBrush(Task.RiskLevel);
    public Brush RiskTintBrush => RiskLevelHelper.GetBadgeTintBrush(Task.RiskLevel);
}
