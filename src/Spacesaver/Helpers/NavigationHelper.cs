using Microsoft.UI.Xaml.Controls;
using Spacesaver.Helpers;
using Spacesaver.Models;
using Spacesaver.Services;

namespace Spacesaver.Helpers;

public static class NavigationHelper
{
    public static void NavigateToTaskList(Page page, TaskCategory? category, string title)
    {
        if (page.Frame is Frame frame)
            frame.Navigate(typeof(Pages.TaskListPage), new TaskListNavigationArgs { Category = category, Title = title });
    }

    public static void NavigateToTaskDetail(Page page, string taskId)
    {
        if (page.Frame is Frame frame)
            frame.Navigate(typeof(Pages.TaskDetailPage), taskId);
    }

    public static void NavigateToRecommendations(Page page)
    {
        if (page.Frame is Frame frame)
            frame.Navigate(typeof(Pages.RecommendationsPage));
    }
}
