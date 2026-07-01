using Microsoft.UI.Xaml.Controls;

using Spacesaver.Helpers;

using Spacesaver.Services;



namespace Spacesaver.Pages;



public sealed partial class SettingsPage : Page

{

    public SettingsPage()

    {

        InitializeComponent();

        Loaded += OnLoaded;

        AppServices.Progress.ProgressChanged += RefreshSummary;

        Unloaded += (_, _) => AppServices.Progress.ProgressChanged -= RefreshSummary;

    }



    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)

    {

        RefreshSummary();

        await AnimationHelper.PlayStaggerEntranceAsync([HeaderPanel, ProgressCard, SafetyCard]);

        AnimationHelper.AttachHoverLift(ProgressCard);

        AnimationHelper.AttachHoverLift(SafetyCard);

    }



    private void RefreshSummary()

    {

        ProgressSummaryText.Text =

            $"{AppServices.Progress.CompletedCount} of {AppServices.Progress.TotalTasks} tasks marked complete";

    }



    private async void ResetProgress_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)

    {

        var dialog = new ContentDialog

        {

            Title = "Reset progress?",

            Content = "This clears all completed task checkmarks. Your files are not affected.",

            PrimaryButtonText = "Reset",

            CloseButtonText = "Cancel",

            DefaultButton = ContentDialogButton.Close,

            XamlRoot = XamlRoot

        };



        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)

        {

            AppServices.Progress.Reset();

            AnimationHelper.Pulse(ProgressCard);

        }

    }

}

