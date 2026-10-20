using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using nvidiaProfileInspector;
using nvidiaProfileInspector.Common;
using nvidiaProfileInspector.Common.Helper;
using nvidiaProfileInspector.Common.Updates;
using nvidiaProfileInspector.Localization;
using nvidiaProfileInspector.Native.NVAPI2;
using nvidiaProfileInspector.Services;
using nvidiaProfileInspector.UI.ViewModels;
using nvidiaProfileInspector.UI.Views;

internal static class Program
{
    private static int _checks;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        _checks++;
    }

    [STAThread]
    private static int Main(string[] args)
    {
        var originalDirectory = Environment.CurrentDirectory;
        var scratch = Path.Combine(Path.GetTempPath(), "npi-localization-checks-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(scratch);
        Environment.CurrentDirectory = scratch;
        // Existing portable settings ensure all persistence stays inside this scratch directory.
        File.WriteAllText("settings.xml", "<UserSettings />");
        try
        {
            // The WPF test executable is the entry assembly; relative application pack URIs
            // must resolve against the product assembly under this external harness.
            typeof(Application).GetField("_resourceAssembly", BindingFlags.NonPublic | BindingFlags.Static)
                .SetValue(null, typeof(App).Assembly);
            if (args.Contains("--render"))
            {
                LayoutChecks.Run(args);
                return 0;
            }
            if (args.Contains("--preview"))
            {
                Preview(args);
                return 0;
            }
            if (!args.Contains("-mock")) throw new Exception("Checks require -mock");
            ResourcesAndConfiguration(originalDirectory);
            ForkAndStartupPreferences();
            Search(originalDirectory);
            _checks += ParameterChecks.Run(originalDirectory);
            Console.WriteLine("PASS: " + _checks + " localization/configuration/search checks; no driver writes.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.GetType().FullName + ": " + ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            return 1;
        }
        finally
        {
            Environment.CurrentDirectory = originalDirectory;
            Directory.Delete(scratch, true);
        }
    }

    private static void ResourcesAndConfiguration(string sourceRoot)
    {
        var resourcesPath = Path.Combine(sourceRoot, "nvidiaProfileInspector", "Localization");
        var neutral = XDocument.Load(Path.Combine(resourcesPath, "Strings.resx")).Root.Elements("data")
            .ToDictionary(x => (string)x.Attribute("name"), x => (string)x.Element("value"));
        var chinese = XDocument.Load(Path.Combine(resourcesPath, "Strings.zh-CN.resx")).Root.Elements("data")
            .ToDictionary(x => (string)x.Attribute("name"), x => (string)x.Element("value"));
        foreach (var pair in neutral)
        {
            Check(chinese.ContainsKey(pair.Key) || pair.Key == "LanguageEnglish", "Missing translation: " + pair.Key);
            if (!chinese.ContainsKey(pair.Key)) continue;
            Func<string, string> placeholders = s => string.Join("|", Regex.Matches(s, @"\{\d+(?:[^}]*)\}")
                .Cast<Match>().Select(x => x.Value).OrderBy(x => x));
            Check(placeholders(pair.Value) == placeholders(chinese[pair.Key]), "Placeholder mismatch: " + pair.Key);
            Localizer.Initialize("en");
            Check(Localizer.Get(pair.Key).Replace("\r\n", "\n") == pair.Value.Replace("\r\n", "\n"), "English resource load: " + pair.Key);
            Localizer.Initialize("zh-CN");
            Check(Localizer.Get(pair.Key).Replace("\r\n", "\n") == chinese[pair.Key].Replace("\r\n", "\n"), "Chinese satellite resource load: " + pair.Key);
            // Validate actual composite formatting, not only the placeholder inventory.
            Localizer.Format(pair.Key, DateTime.Now, DateTime.Now, DateTime.Now);
        }
        Check(Localizer.Get("LanguageEnglish") == "English", "Missing Chinese entry must fall back to neutral English");

        var allSource = string.Join("\n", Directory.GetFiles(Path.Combine(sourceRoot, "nvidiaProfileInspector"), "*.*", SearchOption.AllDirectories)
            .Where(p => (p.EndsWith(".cs") || p.EndsWith(".xaml")) && !p.Contains("\\obj\\") && !p.Contains("\\bin\\"))
            .Select(File.ReadAllText));
        foreach (Match reference in Regex.Matches(allSource, "(?:L\\.(?:Get|Format)\\(\"|loc:Text )([A-Za-z0-9_]+)"))
            Check(neutral.ContainsKey(reference.Groups[1].Value), "Unknown resource reference: " + reference.Groups[1].Value);

        Check(UserSettings.LoadSettings().Language == "zh-CN", "Old configuration defaults to Chinese");
        foreach (var language in new[] { "en", "zh-CN", "EN", "invalid", "", null })
        {
            var settings = UserSettings.LoadSettings();
            settings.Language = language;
            settings.Theme = "MidnightTheme.xaml";
            settings.FavoriteSettingIds.Add(0x10E41E01);
            settings.SaveSettings();
            var restored = UserSettings.LoadSettings();
            Check(restored.Language == Localizer.NormalizeLanguage(language), "Language round-trip: " + language);
            Check(restored.Theme == "MidnightTheme.xaml" && restored.FavoriteSettingIds.Contains(0x10E41E01), "Other preferences preserved");
        }
        File.WriteAllText("settings.xml", "<UserSettings><Language>fr</Language></UserSettings>");
        Check(UserSettings.LoadSettings().Language == "zh-CN", "Unsupported persisted language fallback");
        var preference = UserSettings.LoadSettings();
        preference.Language = "en";
        preference.SaveSettings();
        var culture = CultureInfo.GetCultureInfo("de-DE");
        CultureInfo.CurrentCulture = culture;
        var app = new App();
        Check(Localizer.Language == "en", "Language initialized in App constructor before XAML styles");
        Check(CultureInfo.CurrentCulture.Name == culture.Name, "Numeric culture unchanged");
        Console.WriteLine("Loading WPF resources...");
        app.InitializeComponent();
        var themeManager = new ThemeManager();
        themeManager.SetTheme("DarkTheme.xaml");
        Check(UserSettings.LoadSettings().Language == "en", "Saving theme retains language");
        Console.WriteLine("Checking persisted window preferences...");
        var vm = new MainViewModel(null, null, null, null);
        vm.SaveSettings(10, 10, 1190, 750, WindowState.Normal);
        Check(UserSettings.LoadSettings().Language == "en", "Saving window retains language");
        foreach (var language in new[] { "en", "zh-CN" })
        {
            Localizer.Initialize(language);
            Check(new UpdateChannelOption { Value = "Prerelease", Label = UpdateChannelFormatter.ToDisplayName(UpdateChannel.Prerelease) }.ToString() == UpdateChannelFormatter.ToDisplayName(UpdateChannel.Prerelease), "Custom combo selection label");
            Check(UpdateChannelFormatter.Parse("Prerelease") == UpdateChannel.Prerelease, "Stable channel token");
            Check(UpdateChannelFormatter.ToSettingsValue(UpdateChannel.Prerelease) == "Prerelease", "Stable persisted channel");
            Check(UpdateChannelFormatter.ToDisplayName(UpdateChannel.Prerelease) == (language == "en" ? "Pre-release" : "预发布版"), "Localized channel label");
        }
    }

    private static void ForkAndStartupPreferences()
    {
        Console.WriteLine("Checking fork links and startup preferences...");
        const string repository = "https://github.com/INsincere123/nvidiaProfileInspector_zh";
        var api = typeof(GitHubReleaseClient).GetField("ReleasesUrl", BindingFlags.NonPublic | BindingFlags.Static);
        Check((string)api.GetRawConstantValue() == "https://api.github.com/repos/INsincere123/nvidiaProfileInspector_zh/releases", "Fork update API");
        // LoadSettings returns a new instance when no file exists. Keep the portable
        // file present here so this isolated test never resolves the user's real config.
        var defaults = new UserSettings();
        Check(defaults.DisableUpdateCheck && defaults.DisableSplashScreen, "Fresh configuration disables both startup features");
        File.WriteAllText("settings.xml", "<UserSettings><Language>en</Language><Theme>MidnightTheme.xaml</Theme></UserSettings>");
        var legacy = UserSettings.LoadSettings();
        Check(legacy.DisableUpdateCheck && legacy.DisableSplashScreen, "Missing fields use new defaults");
        Check(legacy.Language == "en" && legacy.Theme == "MidnightTheme.xaml", "Legacy unrelated settings preserved");
        foreach (var disabled in new[] { false, true })
        {
            File.WriteAllText("settings.xml", "<UserSettings><Language>en</Language><Theme>MidnightTheme.xaml</Theme>"
                + "<DisableUpdateCheck>" + disabled.ToString().ToLowerInvariant() + "</DisableUpdateCheck>"
                + "<DisableSplashScreen>" + disabled.ToString().ToLowerInvariant() + "</DisableSplashScreen></UserSettings>");
            var loaded = UserSettings.LoadSettings();
            Check(loaded.DisableUpdateCheck == disabled && loaded.DisableSplashScreen == disabled, "Explicit legacy preferences preserved");
            foreach (var language in new[] { "zh-CN", "en" })
            {
                Localizer.Initialize(language);
                // Supplying an existing release bypasses the dialog's normal network query.
                var dialog = new nvidiaProfileInspector.UI.Views.Dialogs.AboutDialog(
                    new UpdateRelease(new Version(3, 0, 2, 0), false, repository + "/releases", null));
                var vm = (AboutViewModel)dialog.DataContext;
                Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                Check(vm.GitHubRepositoryUrl == repository && vm.GitHubUrl == repository + "/releases", "Fork About links");
                Check(vm.Contributors.Any(x => x.Name == "Orbmu2k" && x.ProfileUrl == "https://github.com/Orbmu2k"), "Original author attribution retained");
                var children = LogicalDescendants(dialog).ToList();
                foreach (var path in new[] { nameof(vm.IsAutomaticUpdateCheckEnabled), nameof(vm.IsSplashScreenDisabled) })
                {
                    var box = children.OfType<System.Windows.Controls.CheckBox>().Single(x =>
                        System.Windows.Data.BindingOperations.GetBinding(x, System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty)?.Path.Path == path);
                    Check(box.IsChecked == (path == nameof(vm.IsAutomaticUpdateCheckEnabled) ? !disabled : disabled), "About checkbox binding: " + path);
                }
                Check(children.OfType<System.Windows.Controls.TextBlock>().Any(x => x.Text == repository.Substring(8)), "Visible repository text matches fork");
                var commands = children.OfType<System.Windows.Controls.Button>().Select(x => x.Command).ToList();
                Check(commands.Contains(vm.OpenGitHubCommand) && commands.Contains(vm.OpenGitHubRepositoryCommand), "Both About link buttons bound");
                vm.IsAutomaticUpdateCheckEnabled = disabled;
                vm.IsSplashScreenDisabled = !disabled;
                var saved = UserSettings.LoadSettings();
                Check(saved.DisableUpdateCheck == !disabled && saved.DisableSplashScreen == !disabled, "About toggles persist on reload");
                Check(saved.Language == "en" && saved.Theme == "MidnightTheme.xaml", "About toggles preserve unrelated preferences");
                new ThemeManager().SetTheme("MidnightTheme.xaml");
                new MainViewModel(null, null, null, null).SaveSettings(10, 10, 1190, 750, WindowState.Normal);
                saved = UserSettings.LoadSettings();
                Check(saved.DisableUpdateCheck == !disabled && saved.DisableSplashScreen == !disabled, "Theme/window saves preserve startup preferences");
                // Restore the original combination for the other language's dialog.
                saved.DisableUpdateCheck = disabled;
                saved.DisableSplashScreen = disabled;
                saved.SaveSettings();
            }
        }
    }

    private static System.Collections.Generic.IEnumerable<DependencyObject> LogicalDescendants(DependencyObject parent)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
        {
            yield return child;
            foreach (var descendant in LogicalDescendants(child)) yield return descendant;
        }
    }

    private static void Search(string sourceRoot)
    {
        var ids = new[] { "CustomSettingNames.xml", "Reference.xml" }.SelectMany(file =>
            XDocument.Load(Path.Combine(sourceRoot, "nvidiaProfileInspector", file)).Descendants("HexSettingID")
                .Select(x => Convert.ToUInt32(x.Value.Substring(2), 16))).ToHashSet();
        var aliases = (IDictionary)typeof(SettingSearch).GetField("ChineseAliases", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        foreach (DictionaryEntry alias in aliases)
            Check(ids.Contains((uint)alias.Key), "Alias ID missing from metadata: " + alias.Key);

        var item = new SettingItemViewModel(new SettingItem
        {
            SettingId = 0x10E41E01, SettingText = "DLSS - Enable DLL Override", AlternateNames = "NGX override",
            GroupName = "05 - Upscaling and Frame Generation", ValueText = "Off", ValueRaw = "0x00000000", State = SettingState.NotAssiged
        });
        Console.WriteLine("Checking both search entry points...");
        var main = new MainViewModel(null, null, null, null);
        var predicate = typeof(MainViewModel).GetMethod("FilterPredicate", BindingFlags.Instance | BindingFlags.NonPublic);
        var list = new SettingsListViewModel();
        list.AddItem(item);
        foreach (var language in new[] { "en", "zh-CN" })
        {
            Localizer.Initialize(language);
            foreach (var query in new[] { "超分辨率", "dlss", "ngx", "0X10E41E01", "", "   " })
            {
                main.FilterText = query;
                Check((bool)predicate.Invoke(main, new object[] { item }), "Main search: " + query);
                list.FilterText = query;
                Check(list.ItemsView.Cast<object>().Count() == 1, "List search: " + query);
            }
            main.FilterText = "不存在";
            Check(!(bool)predicate.Invoke(main, new object[] { item }), "Unmatched main search");
            list.FilterText = "不存在";
            Check(list.ItemsView.IsEmpty, "Unmatched list search");
        }
        main.FilterText = "超分辨率";
        typeof(MainViewModel).GetField("_modifiedOnly", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(main, true);
        Check(!(bool)predicate.Invoke(main, new object[] { item }), "Chinese alias must not bypass modified-only filter");
        Check(!SettingSearch.Matches(0xFFFFFFFF, null, null, null, "帧生成"), "Unknown ID must not inherit aliases");
        Check(item.SelectedValue == "Off" && item.ValueRaw == "0x00000000" && !item.IsModified, "Search never changes raw values or modification state");
        item.SelectedValue = "On";
        Localizer.Initialize("en");
        Check(item.SelectedValue == "On" && item.IsModified, "Language initialization retains unsaved edits");
    }

    // Exercise the actual mock view without invoking startup file-association registration,
    // single-instance messaging, update installation, or any real driver operations.
    private static void Preview(string[] args)
    {
        if (!args.Contains("-mock")) throw new Exception("Preview requires -mock");
        var language = args.Contains("en") ? "en" : "zh-CN";
        var theme = args.FirstOrDefault(x => x.EndsWith("Theme.xaml")) ?? "DarkTheme.xaml";
        var settings = new UserSettings { Language = language, Theme = theme, DisableUpdateCheck = true, Win11BackdropMode = "Disabled" };
        settings.SaveSettings();
        var app = new App();
        app.InitializeComponent();
        if (!NvapiDrsWrapper.Instance.IsMockMode) throw new Exception("Real driver access forbidden in preview");
        var bootstrapper = new AppBootstrapper();
        typeof(App).GetField("_bootstrapper", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, bootstrapper);
        bootstrapper.Initialize();
        bootstrapper.Resolve<ThemeManager>().LoadSavedTheme();
        var window = new MainWindow(disableInitialScan: true);
        app.MainWindow = window;
        window.Closed += (_, __) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        window.Show();
        Dispatcher.Run();
        bootstrapper.Dispose();
    }
}
