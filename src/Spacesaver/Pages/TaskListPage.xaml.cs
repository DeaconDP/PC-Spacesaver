using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Spacesaver.Helpers;
using Spacesaver.Models;
using Spacesaver.Services;
using Spacesaver.ViewModels;

namespace Spacesaver.Pages;

public sealed partial class TaskListPage : Page
{
    private TaskListNavigationArgs? _args;

    public TaskListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        AppServices.Progress.ProgressChanged += OnProgressChanged;
        Unloaded += (_, _) => AppServices.Progress.ProgressChanged -= OnProgressChanged;
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        await AnimationHelper.PlayStaggerEntranceAsync([HeaderText, SubtitleText]);

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _args = e.Parameter as TaskListNavigationArgs ?? new TaskListNavigationArgs();
        HeaderText.Text = _args.Title;
        SubtitleText.Text = _args.Category is null
            ? "All 20 cleanup techniques in recommended order."
            : $"{TaskCatalog.GetByCategory(_args.Category).Count} tasks in this category.";

        if (_args.Category is TaskCategory category)
        {
            var tone = SemanticColorHelper.ForTaskCategory(category);
            HeaderText.Foreground = SemanticColorHelper.GetForegroundBrush(tone);
        }
        else
        {
            HeaderText.ClearValue(TextBlock.ForegroundProperty);
        }

        LoadTasks();
    }

    private void OnProgressChanged() => LoadTasks();

    private void LoadTasks()
    {
        var tasks = TaskCatalog.GetByCategory(_args?.Category);
        var rows = tasks
            .Select(t => new TaskRowViewModel(t, AppServices.Progress.IsCompleted(t.Id)))
            .ToList();
        TaskListView.ItemsSource = rows;
    }

    private void TaskListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is TaskRowViewModel row)
            NavigationHelper.NavigateToTaskDetail(this, row.Task.Id);
    }
}
