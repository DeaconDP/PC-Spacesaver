using Microsoft.UI.Xaml;

using Microsoft.UI.Xaml.Controls;

using Microsoft.UI.Xaml.Media;

using Microsoft.UI.Xaml.Navigation;

using Spacesaver.Helpers;

using Spacesaver.Models;

using Spacesaver.Services;



namespace Spacesaver.Pages;



public sealed partial class TaskDetailPage : Page

{

    private CleanupTask? _task;



    public TaskDetailPage()

    {

        InitializeComponent();

    }



    protected override void OnNavigatedTo(NavigationEventArgs e)

    {

        base.OnNavigatedTo(e);

        if (e.Parameter is not string taskId)

            return;



        _task = TaskCatalog.GetById(taskId);

        if (_task is null)

            return;



        BindTask(_task);

    }



    private async void BindTask(CleanupTask task)

    {

        TaskNumberText.Text = $"Task #{task.Number}";

        TitleText.Text = task.Title;

        DescriptionText.Text = task.Description;

        SavingsText.Text = task.EstimatedSavings;

        RiskText.Text = task.RiskLabel;

        CategoryText.Text = task.CategoryLabel;



        var categoryTone = SemanticColorHelper.ForTaskCategory(task.Category);

        SemanticColorHelper.ApplyChip(SavingsBorder, SavingsText, categoryTone);

        SemanticColorHelper.ApplyChip(
            RiskBorder,
            RiskText,
            SemanticColorHelper.ForRiskLevel(task.RiskLevel));

        SemanticColorHelper.ApplyChip(CategoryBorder, CategoryText, categoryTone);



        AdminInfoBar.IsOpen = task.RequiresAdmin;

        if (AdminInfoBar.IsOpen)

            AnimationHelper.FadeSlideIn(AdminInfoBar);



        if (!string.IsNullOrWhiteSpace(task.Warning))

        {

            WarningInfoBar.Message = task.Warning;

            WarningInfoBar.IsOpen = true;

            AnimationHelper.FadeSlideIn(WarningInfoBar);

        }

        else

        {

            WarningInfoBar.IsOpen = false;

        }



        StepsList.Items.Clear();

        var stepPanels = new List<UIElement>();

        var stepTone = categoryTone;

        var stepAccent = SemanticColorHelper.GetAccentColor(stepTone);

        var stepTint = SemanticColorHelper.GetTintColor(stepTone);



        for (var i = 0; i < task.Steps.Count; i++)

        {

            var panel = new StackPanel

            {

                Orientation = Orientation.Horizontal,

                Spacing = 12,

                Margin = new Thickness(0, 0, 0, 10)

            };



            var number = new Border

            {

                Width = 28,

                Height = 28,

                CornerRadius = new CornerRadius(14),

                Background = new SolidColorBrush(stepTint),

                BorderBrush = new SolidColorBrush(stepAccent),

                BorderThickness = new Thickness(1),

                Child = new TextBlock

                {

                    Text = (i + 1).ToString(),

                    HorizontalAlignment = HorizontalAlignment.Center,

                    VerticalAlignment = VerticalAlignment.Center,

                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,

                    Foreground = new SolidColorBrush(stepAccent)

                }

            };



            var text = new TextBlock

            {

                Text = task.Steps[i],

                TextWrapping = TextWrapping.WrapWholeWords,

                VerticalAlignment = VerticalAlignment.Center,

                MaxWidth = 640

            };



            panel.Children.Add(number);

            panel.Children.Add(text);

            StepsList.Items.Add(panel);

            stepPanels.Add(panel);

        }



        if (task.PrimaryAction is not null)

        {

            OpenToolButton.Content = task.PrimaryAction.ButtonLabel;

            OpenToolButton.Visibility = Visibility.Visible;

        }

        else

        {

            OpenToolButton.Visibility = Visibility.Collapsed;

        }



        CopyCommandButton.Visibility = string.IsNullOrWhiteSpace(task.CopyCommand)

            ? Visibility.Collapsed

            : Visibility.Visible;



        DoneToggle.IsOn = AppServices.Progress.IsCompleted(task.Id);



        await AnimationHelper.PlayStaggerEntranceAsync([

            TaskHeaderPanel,

            StepsPanel,

            ActionPanel,

            DoneToggle

        ]);



        await AnimationHelper.PlayStaggerEntranceAsync(stepPanels, staggerMs: 45);

        AnimationHelper.AttachHoverLift(OpenToolButton, hoverScale: 1.02f);

    }



    private async void OpenToolButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)

    {

        if (_task?.PrimaryAction is null)

            return;



        AnimationHelper.Pulse(OpenToolButton);

        var launched = await AppServices.Launcher.LaunchAsync(_task.PrimaryAction);

        if (!launched)

        {

            var dialog = new ContentDialog

            {

                Title = "Could not open tool",

                Content = "Try opening the tool manually from Start or Settings.",

                CloseButtonText = "OK",

                XamlRoot = XamlRoot

            };

            await dialog.ShowAsync();

        }

    }



    private async void CopyCommandButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)

    {

        if (_task?.CopyCommand is null)

            return;



        AnimationHelper.Pulse(CopyCommandButton);

        await WindowsLauncherService.CopyToClipboardAsync(_task.CopyCommand);



        var dialog = new ContentDialog

        {

            Title = "Command copied",

            Content = "Paste into an Administrator PowerShell or Terminal window.",

            CloseButtonText = "OK",

            XamlRoot = XamlRoot

        };

        await dialog.ShowAsync();

    }



    private void DoneToggle_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)

    {

        if (_task is null)

            return;



        AppServices.Progress.SetCompleted(_task.Id, DoneToggle.IsOn);



        if (DoneToggle.IsOn)

            AnimationHelper.Pulse(DoneToggle, peakScale: 1.03f);

    }

}
