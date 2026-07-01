namespace Spacesaver.Models;

public sealed class LaunchAction
{
    public LaunchActionType Type { get; init; }
    public string Target { get; init; } = string.Empty;
    public string? Arguments { get; init; }
    public string ButtonLabel { get; init; } = "Open Tool";

    public static LaunchAction Settings(string uri, string label = "Open Settings") =>
        new() { Type = LaunchActionType.SettingsUri, Target = uri, ButtonLabel = label };

    public static LaunchAction Executable(string path, string? args = null, string label = "Open Tool") =>
        new() { Type = LaunchActionType.Executable, Target = path, Arguments = args, ButtonLabel = label };

    public static LaunchAction Explorer(string path, string label = "Open Folder") =>
        new() { Type = LaunchActionType.ExplorerPath, Target = path, ButtonLabel = label };

    public static LaunchAction ControlPanel(string executable, string label = "Open Control Panel") =>
        new() { Type = LaunchActionType.ControlPanel, Target = executable, ButtonLabel = label };

    public static LaunchAction Browser(string uri, string label = "Open Browser Settings") =>
        new() { Type = LaunchActionType.BrowserUri, Target = uri, ButtonLabel = label };
}
