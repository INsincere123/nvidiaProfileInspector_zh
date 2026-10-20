using L = nvidiaProfileInspector.Localization.Localizer;
namespace nvidiaProfileInspector.UI.Views.Dialogs
{
    using nvidiaProfileInspector.Common;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Asks the user how an interactively triggered .nip import should be applied
    /// (drag &amp; drop or file-association double-click). Returns null when cancelled.
    /// </summary>
    public static class ProfileImportModePrompt
    {
        public static ProfileImportMode? Ask(Window owner, int fileCount)
        {
            var items = new List<ActionCardsDialog.ActionCardItem>
            {
                new ActionCardsDialog.ActionCardItem
                {
                    Title = L.Get("Replace_a7cf7b"),
                    Description = L.Get("Reset_each_target_0_and_write_011684"),
                    IconPath = Application.Current.Resources["IconImport"] as Geometry,
                    IconFill = Application.Current.Resources["TextSecondaryBrush"] as Brush,
                },
                new ActionCardsDialog.ActionCardItem
                {
                    Title = L.Get("Merge_ea8f0d"),
                    Description = L.Get("Keep_the_existing_values_in_each_708f79"),
                    IconPath = Application.Current.Resources["IconUser"] as Geometry,
                    IconFill = Application.Current.Resources["TextSecondaryBrush"] as Brush,
                },
            };

            var subtitle = fileCount == 1
                ? L.Get("Choose_how_this_nip_file_should_5ac068")
                : L.Format("Choose_how_these_0_nip_files_44e5a4", fileCount);

            var dialog = new ActionCardsDialog(L.Get("Import_Profiles_c97e7b"), items, subtitle);
            if (owner != null && owner.IsLoaded)
                dialog.Owner = owner;
            else
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (dialog.ShowDialog() != true || dialog.SelectedItem == null)
                return null;

            return ReferenceEquals(dialog.SelectedItem, items[1])
                ? ProfileImportMode.Merge
                : ProfileImportMode.Replace;
        }
    }
}
