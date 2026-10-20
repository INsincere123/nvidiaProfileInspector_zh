using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using nvidiaProfileInspector;
using nvidiaProfileInspector.Common.Helper;
using nvidiaProfileInspector.Common;
using nvidiaProfileInspector.Common.CustomSettings;
using nvidiaProfileInspector.Common.Meta;
using nvidiaProfileInspector.Common.Updates;
using nvidiaProfileInspector.Localization;
using nvidiaProfileInspector.Native.NVAPI2;
using nvidiaProfileInspector.Services;
using nvidiaProfileInspector.UI.ViewModels;
using nvidiaProfileInspector.UI.Views;
using nvidiaProfileInspector.UI.Views.Dialogs;

internal static class LayoutChecks
{
    // Offline WPF rendering at 96/144 DPI; does not alter Windows display scaling.
    public static void Run(string[] args)
    {
        if (!args.Contains("-mock")) throw new Exception("Layout checks require -mock");
        var language = args.Contains("en") ? "en" : "zh-CN";
        new UserSettings { Language = language, DisableUpdateCheck = true }.SaveSettings();
        var app = new App();
        app.InitializeComponent();
        if (!NvapiDrsWrapper.Instance.IsMockMode) throw new Exception("Real driver access forbidden");
        var bootstrapper = new AppBootstrapper();
        typeof(App).GetField("_bootstrapper", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, bootstrapper);
        bootstrapper.Initialize();
        var output = Path.Combine(AppContext.BaseDirectory, "previews", language);
        Directory.CreateDirectory(output);
        var count = 0;
        foreach (var theme in new[] { "DarkTheme.xaml", "MidnightTheme.xaml", "SlateLightTheme.xaml", "CleanWhiteTheme.xaml" })
        {
            bootstrapper.Resolve<ThemeManager>().SetTheme(theme);
            var windows = new Dictionary<string, Window>
            {
                ["Main"] = new MainWindow(disableInitialScan: true) { DataContext = ParameterPreview() },
                ["MainExtended"] = new MainWindow(disableInitialScan: true) { DataContext = ParameterPreview(true) },
                ["Splash"] = new SplashWindow(),
                ["Message"] = new MessageBoxView { DataContext = new MessageBoxViewModel(Localizer.Get("LanguageRestartMessage"), Localizer.Get("LanguageRestartTitle"), MessageBoxButton.YesNoCancel, MessageBoxImage.Information) },
                ["Input"] = new InputDialog(Localizer.Get("Create_Profile_dccc9c"), Localizer.Get("Enter_a_unique_name_for_your_b3deab"), "Sample profile", true),
                ["Actions"] = new ActionCardsDialog(Localizer.Get("Export_Profiles_6f6f72"), new[]
                {
                    new ActionCardsDialog.ActionCardItem { Title = Localizer.Get("Export_current_profile_only_a307ef"), Description = Localizer.Get("Export_only_the_settings_you_have_c88cfc") }
                }),
                ["About"] = new AboutDialog(new UpdateRelease(new Version(3, 0, 2, 0), false, "", null)),
                ["Bits"] = new BitEditorDialog()
            };
            foreach (var pair in windows)
            {
                var window = pair.Value;
                var root = (FrameworkElement)window.Content;
                var width = double.IsNaN(window.Width) ? Math.Max(600, window.MinWidth) : window.Width;
                var height = window.SizeToContent == SizeToContent.Height ? double.PositiveInfinity
                    : double.IsNaN(window.Height) ? Math.Max(400, window.MinHeight) : window.Height;
                root.Measure(new Size(width, height));
                if (double.IsPositiveInfinity(height)) height = root.DesiredSize.Height;
                root.Arrange(new Rect(0, 0, width, height));
                root.UpdateLayout();
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                root.UpdateLayout();
                foreach (var scale in new[] { 1.0, 1.5 })
                {
                    var bitmap = new RenderTargetBitmap((int)Math.Ceiling(width * scale), (int)Math.Ceiling(height * scale),
                        96 * scale, 96 * scale, PixelFormats.Pbgra32);
                    bitmap.Render(root);
                    var png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(bitmap));
                    var filename = theme.Replace(".xaml", "") + "-" + pair.Key + "-" + (scale == 1 ? "100" : "150") + ".png";
                    using (var stream = File.Create(Path.Combine(output, filename))) png.Save(stream);
                    count++;
                }
            }
        }
        bootstrapper.Dispose();
        Console.WriteLine("Rendered " + count + " offline WPF layouts for " + language + ": " + output);
    }
    private static MainViewModel ParameterPreview(bool extended = false)
    {
        var source = CustomSettingNames.FactoryLoadFromString(EmbeddedResourceHelper.GetFileAsString("nvidiaProfileInspector.CustomSettingNames.xml"));
        var meta = new CustomSettingMetaService(source);
        var ids = extended ? new uint[] { 0x00432F84, 0x00DD48FB, 0x00AC8497, 0x000F00FF, 0x1094F1F7, 0x10D773D2, 0x50166C5E, 0x20324987 } : new uint[] { 0x10E41E01, 0x10E41E04, 0x104596A2, 0x1057EB71, 0x00CE2691, 0x00A879CF };
        var model = new MainViewModel(null, null, null, null);
        model.Settings.Clear();
        foreach (var setting in source.Settings.Where(x => ids.Contains(x.SettingId)).OrderBy(x => x.GroupName).ThenBy(x => x.UserfriendlyName))
        {
            var qwords = setting.FallbackType == "QWORD" ? meta.GetQwordValues(setting.SettingId) : null;
            var values = qwords == null ? meta.GetDwordValues(setting.SettingId) : null;
            model.Settings.Add(new SettingItemViewModel(new SettingItem
            {
                SettingId = setting.SettingId, SettingText = setting.UserfriendlyName, GroupName = setting.GroupName,
                ValueText = qwords?.LastOrDefault()?.ValueName ?? values?.FirstOrDefault()?.ValueName ?? "0x00000000", State = SettingState.NotAssiged
            }) { DwordValues = values, QwordValues = qwords });
        }
        model.FilterText = " "; // Publish group headers after adding the source-derived rows.
        var descriptionId = extended ? 0x00432F84u : 0x10E41E01u;
        model.SettingDescription = ParameterLocalizer.Description(descriptionId, source.Settings.Single(x => x.SettingId == descriptionId).Description);
        return model;
    }

}
