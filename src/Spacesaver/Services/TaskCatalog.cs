using Spacesaver.Models;

namespace Spacesaver.Services;

public static class TaskCatalog
{
    public static IReadOnlyList<CleanupTask> All { get; } = BuildTasks();

    public static CleanupTask? GetById(string id) =>
        All.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public static IReadOnlyList<CleanupTask> GetByCategory(TaskCategory? category) =>
        category is null ? All : All.Where(t => t.Category == category).ToList();

    private static List<CleanupTask> BuildTasks() =>
    [
        new()
        {
            Id = "storage-sense",
            Number = 1,
            Title = "Enable Storage Sense",
            Description = "Turn on automatic cleanup so Windows removes temporary files and old Recycle Bin items in the background.",
            Category = TaskCategory.QuickWins,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "Ongoing",
            IconGlyph = "\uE7C0",
            Steps =
            [
                "Open Storage Sense settings using the button below.",
                "Turn Storage Sense on.",
                "Set how often it runs (recommended: during low disk space or weekly).",
                "Enable cleanup for temporary files and Recycle Bin.",
                "Leave Downloads cleanup off unless you are sure nothing important stays there."
            ],
            PrimaryAction = LaunchAction.Settings("ms-settings:storagepolicies", "Open Storage Sense")
        },
        new()
        {
            Id = "temp-files-settings",
            Number = 2,
            Title = "Remove Temporary Files (Settings)",
            Description = "Use the built-in Temporary files screen to remove caches, update leftovers, and other safe junk.",
            Category = TaskCategory.QuickWins,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "1–40 GB",
            IconGlyph = "\uE74C",
            Steps =
            [
                "Open Storage settings using the button below.",
                "Select Temporary files (or your main drive, then Temporary files).",
                "Review each category and check everything except Downloads unless you intend to clear that folder.",
                "Click Remove files and wait for cleanup to finish."
            ],
            PrimaryAction = LaunchAction.Settings("ms-settings:storagesense", "Open Storage Settings")
        },
        new()
        {
            Id = "recycle-bin",
            Number = 3,
            Title = "Empty Recycle Bin",
            Description = "Deleted files stay in the Recycle Bin until you empty it. This is one of the fastest ways to recover space.",
            Category = TaskCategory.QuickWins,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "Variable",
            IconGlyph = "\uE74D",
            Steps =
            [
                "Open the Recycle Bin using the button below.",
                "Review contents if you might need something back.",
                "Right-click Recycle Bin on the desktop and choose Empty Recycle Bin, or select all items and delete.",
                "Confirm when prompted."
            ],
            PrimaryAction = LaunchAction.Explorer("shell:RecycleBinFolder", "Open Recycle Bin")
        },
        new()
        {
            Id = "user-temp",
            Number = 4,
            Title = "Clear User Temp Folder",
            Description = "Programs often leave temporary files in your user Temp folder. Files not modified recently are usually safe to remove.",
            Category = TaskCategory.QuickWins,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "100 MB–3 GB",
            IconGlyph = "\uE8B7",
            Steps =
            [
                "Close open applications to avoid file-in-use errors.",
                "Open your Temp folder using the button below.",
                "Press Ctrl+A to select all, then Delete.",
                "Skip files Windows says are in use — that is normal.",
                "Empty the Recycle Bin afterward if files were moved there."
            ],
            PrimaryAction = LaunchAction.Explorer("%TEMP%", "Open Temp Folder")
        },
        new()
        {
            Id = "disk-cleanup",
            Number = 5,
            Title = "Run Disk Cleanup (Standard)",
            Description = "Disk Cleanup removes temporary files, Recycle Bin contents, and other user-level clutter without admin rights.",
            Category = TaskCategory.QuickWins,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "500 MB–5 GB",
            IconGlyph = "\uE9F5",
            Steps =
            [
                "Launch Disk Cleanup using the button below.",
                "Select your system drive (usually C:) if prompted.",
                "Wait for the scan to finish.",
                "Check Temporary files, Recycle Bin, and Thumbnails as needed.",
                "Click OK, then Delete Files to confirm."
            ],
            PrimaryAction = LaunchAction.Executable("cleanmgr.exe", "/d C:", "Run Disk Cleanup")
        },
        new()
        {
            Id = "disk-cleanup-system",
            Number = 6,
            Title = "Disk Cleanup: Clean Up System Files",
            Description = "The hidden \"Clean up system files\" button unlocks Windows Update leftovers, delivery optimization caches, and more.",
            Category = TaskCategory.SystemCleanup,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "1–10 GB",
            IconGlyph = "\uE7EF",
            RequiresAdmin = true,
            Warning = "This step requires Administrator privileges. Run Disk Cleanup as admin and click \"Clean up system files\" to see the full list.",
            Steps =
            [
                "Launch Disk Cleanup using the button below.",
                "When the dialog appears, click \"Clean up system files\" (not just OK).",
                "If prompted for admin approval, allow it.",
                "Wait for the second scan — it finds much more than the standard scan.",
                "Review categories carefully, then click OK to delete selected items."
            ],
            PrimaryAction = LaunchAction.Executable("cleanmgr.exe", "/d C:", "Run Disk Cleanup")
        },
        new()
        {
            Id = "windows-update-cleanup",
            Number = 7,
            Title = "Windows Update Cleanup",
            Description = "Old update files kept for rollback can consume several gigabytes. Safe to remove once recent updates are stable.",
            Category = TaskCategory.SystemCleanup,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "1–8 GB",
            IconGlyph = "\uE895",
            RequiresAdmin = true,
            Warning = "Only remove this if you do not plan to uninstall a recent Windows update.",
            Steps =
            [
                "Run Disk Cleanup as administrator (see task 6).",
                "Click \"Clean up system files\" and wait for the scan.",
                "Find and check \"Windows Update Cleanup\".",
                "Click OK and confirm deletion.",
                "Wait — this step can take several minutes."
            ],
            PrimaryAction = LaunchAction.Executable("cleanmgr.exe", "/d C:", "Run Disk Cleanup")
        },
        new()
        {
            Id = "delivery-optimization",
            Number = 8,
            Title = "Delivery Optimization Files",
            Description = "Windows caches update files to share with other PCs on your network. These caches are safe to delete.",
            Category = TaskCategory.SystemCleanup,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "500 MB–2 GB",
            IconGlyph = "\uE896",
            RequiresAdmin = true,
            Steps =
            [
                "Run Disk Cleanup with \"Clean up system files\" (see task 6).",
                "Check \"Delivery Optimization Files\".",
                "Click OK to remove the cached update files."
            ],
            PrimaryAction = LaunchAction.Executable("cleanmgr.exe", "/d C:", "Run Disk Cleanup")
        },
        new()
        {
            Id = "driver-packages",
            Number = 9,
            Title = "Old Device Driver Packages",
            Description = "Windows keeps previous driver versions so older hardware can still work. Remove only if you rarely connect legacy devices.",
            Category = TaskCategory.SystemCleanup,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "200 MB–1 GB",
            IconGlyph = "\uE964",
            RequiresAdmin = true,
            Warning = "You may need these packages if you reconnect older printers, scanners, or USB devices.",
            Steps =
            [
                "Run Disk Cleanup with \"Clean up system files\".",
                "Check \"Device driver packages\".",
                "Click OK to remove superseded driver packages."
            ],
            PrimaryAction = LaunchAction.Executable("cleanmgr.exe", "/d C:", "Run Disk Cleanup")
        },
        new()
        {
            Id = "windows-temp",
            Number = 10,
            Title = "Clear Windows Temp Folder",
            Description = "The system Temp folder under C:\\Windows\\Temp holds installer leftovers. Requires administrator access.",
            Category = TaskCategory.SystemCleanup,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "100 MB–1 GB",
            IconGlyph = "\uE8B7",
            RequiresAdmin = true,
            Warning = "Do not delete anything outside the Temp folder. Never modify other folders under C:\\Windows manually.",
            Steps =
            [
                "Open File Explorer as Administrator (right-click Start → Terminal (Admin), then run: explorer C:\\Windows\\Temp).",
                "Select files and folders inside Temp only.",
                "Delete items that are not in use.",
                "Skip locked files — Windows may still be using them."
            ],
            PrimaryAction = LaunchAction.Explorer("C:\\Windows\\Temp", "Open Windows Temp")
        },
        new()
        {
            Id = "windows-old",
            Number = 11,
            Title = "Previous Windows Installation",
            Description = "After a major upgrade, Windows keeps a full copy of the old install (Windows.old) so you can roll back.",
            Category = TaskCategory.LargeSpaceWins,
            RiskLevel = RiskLevel.Advanced,
            EstimatedSavings = "10–30 GB",
            IconGlyph = "\uE7E8",
            RequiresAdmin = true,
            Warning = "Deleting this removes your ability to roll back to the previous Windows version. Only proceed if your system has been stable for several weeks.",
            Steps =
            [
                "Open Storage settings or Disk Cleanup with system files.",
                "Look for \"Previous Windows installation(s)\" or \"Windows.old\".",
                "Confirm you do not need to roll back your upgrade.",
                "Select the category and remove files.",
                "Alternatively: Settings → System → Storage → Temporary files → Previous Windows installation."
            ],
            PrimaryAction = LaunchAction.Settings("ms-settings:storagesense", "Open Storage Settings")
        },
        new()
        {
            Id = "dism-cleanup",
            Number = 12,
            Title = "DISM Component Store Cleanup",
            Description = "Safely shrinks the WinSxS component store. Never delete WinSxS files manually — use DISM instead.",
            Category = TaskCategory.LargeSpaceWins,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "2–5 GB",
            IconGlyph = "\uE8FD",
            RequiresAdmin = true,
            CopyCommand = "DISM /Online /Cleanup-Image /StartComponentCleanup",
            Steps =
            [
                "Copy the DISM command using the button below.",
                "Open Terminal or PowerShell as Administrator.",
                "Paste the command and press Enter.",
                "Wait several minutes for cleanup to complete.",
                "Restart your PC when finished."
            ],
            PrimaryAction = LaunchAction.Executable("wt.exe", "-p PowerShell", "Open Terminal")
        },
        new()
        {
            Id = "hibernation",
            Number = 13,
            Title = "Manage Hibernation File",
            Description = "hiberfil.sys stores hibernation and Fast Startup data. Disabling or shrinking it can free significant space.",
            Category = TaskCategory.LargeSpaceWins,
            RiskLevel = RiskLevel.Advanced,
            EstimatedSavings = "2–16 GB",
            IconGlyph = "\uE708",
            RequiresAdmin = true,
            CopyCommand = "powercfg /hibernate off",
            Warning = "Disabling hibernation also disables Fast Startup. To keep Fast Startup but use less space, use: powercfg /h /type reduced",
            Steps =
            [
                "Decide: disable hibernation entirely, or use a reduced hiberfil.sys.",
                "Copy the command below (off = most space; reduced = keeps Fast Startup).",
                "Open Terminal as Administrator.",
                "Run: powercfg /hibernate off  — OR —  powercfg /h /type reduced",
                "Check free space on C: after completion."
            ],
            PrimaryAction = LaunchAction.Executable("wt.exe", "-p PowerShell", "Open Terminal")
        },
        new()
        {
            Id = "page-file",
            Number = 14,
            Title = "Optimize Virtual Memory (Page File)",
            Description = "Windows may allocate a large pagefile.sys equal to your RAM. A custom size can reclaim gigabytes on high-RAM systems.",
            Category = TaskCategory.LargeSpaceWins,
            RiskLevel = RiskLevel.Advanced,
            EstimatedSavings = "4–16 GB",
            IconGlyph = "\uE8F1",
            Warning = "Do not disable the page file entirely. For 16 GB RAM, try 4096 MB initial and 8192 MB maximum as a starting point.",
            Steps =
            [
                "Open Advanced System Settings using the button below.",
                "Under Performance, click Settings → Advanced tab → Virtual memory → Change.",
                "Uncheck \"Automatically manage paging file size for all drives\".",
                "Select C:, choose Custom size, and enter initial/max values in MB.",
                "Click Set, then OK, and restart when prompted."
            ],
            PrimaryAction = LaunchAction.ControlPanel("SystemPropertiesAdvanced.exe", "Open System Properties")
        },
        new()
        {
            Id = "system-restore",
            Number = 15,
            Title = "Manage System Restore / Shadow Copies",
            Description = "Restore points and shadow copies protect against bad changes but can use substantial disk space over time.",
            Category = TaskCategory.LargeSpaceWins,
            RiskLevel = RiskLevel.Advanced,
            EstimatedSavings = "1–10 GB",
            IconGlyph = "\uE777",
            Warning = "Reducing restore point storage means fewer recovery options if something goes wrong. Keep at least one recent restore point.",
            Steps =
            [
                "Open System Protection settings using the button below.",
                "Select your system drive and click Configure.",
                "Review current disk usage for restore points.",
                "Click Delete to remove all restore points, or reduce Max Usage slider.",
                "Confirm only if you understand the recovery tradeoff."
            ],
            PrimaryAction = LaunchAction.ControlPanel("SystemPropertiesProtection.exe", "Open System Protection")
        },
        new()
        {
            Id = "uninstall-apps",
            Number = 16,
            Title = "Uninstall Unused Applications",
            Description = "Games, creative suites, and forgotten installers often consume the most space. Removing what you do not use is high impact.",
            Category = TaskCategory.ManualReview,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "Variable",
            IconGlyph = "\uE7B8",
            Steps =
            [
                "Open Installed apps using the button below.",
                "Sort by size if available, or scroll to find large apps.",
                "Uninstall programs you no longer need.",
                "Restart if prompted by the uninstaller."
            ],
            PrimaryAction = LaunchAction.Settings("ms-settings:appsfeatures", "Open Installed Apps")
        },
        new()
        {
            Id = "browser-cache",
            Number = 17,
            Title = "Clear Browser Caches",
            Description = "Browsers accumulate cached images, cookies, and site data that can grow to several gigabytes.",
            Category = TaskCategory.ManualReview,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "500 MB–5 GB",
            IconGlyph = "\uE774",
            Steps =
            [
                "Open your browser's privacy or history settings.",
                "For Edge: Settings → Privacy, search, and services → Clear browsing data.",
                "For Chrome: Settings → Privacy and security → Clear browsing data.",
                "Select Cached images and files (and optionally cookies if you accept signing out of sites).",
                "Choose All time for maximum space recovery."
            ],
            PrimaryAction = LaunchAction.Browser("microsoft-edge:settings/clearBrowsingData", "Open Edge Settings")
        },
        new()
        {
            Id = "onedrive",
            Number = 18,
            Title = "OneDrive / Cloud Storage",
            Description = "Cloud sync can fill your local disk with copies of online files. Free up space by using online-only files.",
            Category = TaskCategory.ManualReview,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "Variable",
            IconGlyph = "\uE753",
            Warning = "Online-only files require internet to open. Make sure important files are synced before changing settings.",
            Steps =
            [
                "Open OneDrive settings using the button below.",
                "Review how much local space OneDrive uses.",
                "Enable \"Save space and download files as you use them\" if available.",
                "Right-click large folders in File Explorer → Free up space for items you rarely need offline."
            ],
            PrimaryAction = LaunchAction.Settings("ms-settings:storagesense", "Open Storage Settings")
        },
        new()
        {
            Id = "storage-map",
            Number = 19,
            Title = "Find Large Files (Storage Map)",
            Description = "When you are not sure what is using space, the Storage map shows which categories and folders are largest.",
            Category = TaskCategory.ManualReview,
            RiskLevel = RiskLevel.Safe,
            EstimatedSavings = "Variable",
            IconGlyph = "\uE9D2",
            Steps =
            [
                "Open Storage settings using the button below.",
                "Click your main drive (usually C:).",
                "Review categories: Apps, Temporary files, Documents, Pictures, etc.",
                "Click each category to drill down into large items.",
                "Remove or move files you no longer need — do not delete unknown system folders."
            ],
            PrimaryAction = LaunchAction.Settings("ms-settings:storagesense", "Open Storage Settings")
        },
        new()
        {
            Id = "downloads",
            Number = 20,
            Title = "Review Downloads Folder",
            Description = "Installers, archives, and old documents often pile up in Downloads. Review before deleting — nothing here is recycled automatically by most tools.",
            Category = TaskCategory.ManualReview,
            RiskLevel = RiskLevel.Caution,
            EstimatedSavings = "Variable",
            IconGlyph = "\uE896",
            Warning = "Do not bulk-delete Downloads. Many important files (installers, exports, attachments) live here permanently.",
            Steps =
            [
                "Open your Downloads folder using the button below.",
                "Sort by Size or Date modified to find old large files.",
                "Move keepers to Documents or an external drive.",
                "Delete installers and duplicates you no longer need.",
                "Empty Recycle Bin after cleanup."
            ],
            PrimaryAction = LaunchAction.Explorer("%USERPROFILE%\\Downloads", "Open Downloads")
        }
    ];
}
