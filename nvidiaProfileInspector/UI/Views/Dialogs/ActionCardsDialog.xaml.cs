using L = nvidiaProfileInspector.Localization.Localizer;
using Microsoft.Win32;
using nvidiaProfileInspector.TinyIoc;
using nvidiaProfileInspector.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace nvidiaProfileInspector.UI.Views.Dialogs
{
    public partial class ActionCardsDialog : Window
    {
        public class ActionCardItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public Geometry IconPath { get; set; }
            public Brush IconFill { get; set; }
            public Action Command { get; set; }
            public bool ShowProfileSelection { get; set; }
        }

        private ExportProfilesViewModel _selectionViewModel;
        private string _originalTitle;
        private string _originalSubtitle;

        public ActionCardItem SelectedItem { get; private set; }

        public ActionCardsDialog(string title, IEnumerable<ActionCardItem> items, string subtitle = null)
        {
            InitializeComponent();
            _originalTitle = title;
            _originalSubtitle = subtitle;

            this.Title = title;
            this.CardsContainer.ItemsSource = items;

            if (!string.IsNullOrEmpty(subtitle))
            {
                this.SubtitleTextBlock.Text = subtitle;
                this.SubtitleTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ShowCardsView();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is ActionCardItem item)
            {
                if (item.ShowProfileSelection)
                {
                    ShowProfileSelectionView();
                }
                else
                {
                    SelectedItem = item;
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void ShowCardsView()
        {
            // Restore original title and subtitle
            this.Title = _originalTitle;
            this.SubtitleTextBlock.Text = _originalSubtitle;
            this.SubtitleTextBlock.Visibility = string.IsNullOrEmpty(_originalSubtitle) ? Visibility.Collapsed : Visibility.Visible;

            // Switch views
            this.CardsContainer.Visibility = Visibility.Visible;
            this.SelectionContainer.Visibility = Visibility.Collapsed;
            this.SelectionFooter.Visibility = Visibility.Collapsed;
            this.BackButton.Visibility = Visibility.Collapsed;

            // Clear selection state if needed
            SelectedItem = null;
        }

        private void ShowProfileSelectionView()
        {
            this.Title = L.Get("SelectProfiles");
            this.SubtitleTextBlock.Text = L.Get("Please_choose_the_profiles_you_want_95fed3");
            this.SubtitleTextBlock.Visibility = Visibility.Visible;

            if (_selectionViewModel == null)
            {
                _selectionViewModel = TinyIoC.Resolve<ExportProfilesViewModel>();
                this.ProfilesListBox.ItemsSource = _selectionViewModel.Profiles;
            }

            this.CardsContainer.Visibility = Visibility.Collapsed;
            this.SelectionContainer.Visibility = Visibility.Visible;
            this.SelectionFooter.Visibility = Visibility.Visible;
            this.BackButton.Visibility = Visibility.Visible;

            // Reset the IncludePredefinedCheckBox state
            IncludePredefinedCheckBox.IsChecked = _selectionViewModel.IncludePredefined;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            _selectionViewModel.SelectAll();
        }

        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {
            _selectionViewModel.SelectNone();
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            _selectionViewModel.InvertSelection();
        }

        private void ExportSelected_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectionViewModel.HasSelectedProfiles)
            {
                if (Owner is MainWindow mainWindow && mainWindow.DataContext is MainViewModel mainViewModel)
                    mainViewModel.ShowSnackbar(L.Get("Please_select_at_least_one_profile_081542"), "Warning");
                return;
            }

            _selectionViewModel.IncludePredefined = IncludePredefinedCheckBox.IsChecked == true;

            var dialog = new SaveFileDialog
            {
                Filter = L.Get("NVIDIA_PROFILE_INSPECTOR_Profiles_nip_12629c"),
                DefaultExt = "*.nip",
                FileName = ProfileExportFileNames.ForSelectedProfiles()
            };

            if (dialog.ShowDialog() == true)
            {
                _selectionViewModel.ExportSelectedProfiles(dialog.FileName);

                if (Owner is MainWindow mainWindow && mainWindow.DataContext is MainViewModel mainViewModel)
                    mainViewModel.ShowSnackbar(L.Get("All_selected_profiles_exported_successfully_e68537"), "Success");

                DialogResult = true;
                Close();
            }
        }
    }
}
