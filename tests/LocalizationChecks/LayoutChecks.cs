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
                ["Main"] = new MainWindow(disableInitialScan: true) { DataContext = new MainViewModel(null, null, null, null) },
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
}
