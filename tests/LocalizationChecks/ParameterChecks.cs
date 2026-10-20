using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml.Linq;
using nvidiaProfileInspector.Common;
using nvidiaProfileInspector.Common.CustomSettings;
using nvidiaProfileInspector.Common.Helper;
using nvidiaProfileInspector.Common.Meta;
using nvidiaProfileInspector.Localization;
using nvidiaProfileInspector.Native.NVAPI2;
using nvidiaProfileInspector.UI.Controls;
using nvidiaProfileInspector.UI.ViewModels;

internal static class ParameterChecks
{
    private static int _checks;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        _checks++;
    }

    public static int Run(string sourceRoot)
    {
        var root = Path.Combine(sourceRoot, "nvidiaProfileInspector");
        var catalog = XDocument.Load(Path.Combine(root, "Localization", "ParameterTranslations.zh-CN.xml"));
        var settings = catalog.Root.Element("Settings").Elements("Setting").ToList();
        Check(settings.Count == 49, "Expected the agreed 49 translated settings");
        Check(settings.Select(x => (string)x.Attribute("id")).Distinct().Count() == 49, "Duplicate setting translation ID");
        var files = new[] { "CustomSettingNames.xml", "Reference.xml" }.ToDictionary(x => x,
            x => CustomSettingNames.FactoryLoadFromFile(Path.Combine(root, x)));
        var metadata = files.ToDictionary(x => x.Key, x => new CustomSettingMetaService(x.Value,
            x.Key == "Reference.xml" ? SettingMetaSource.ReferenceSettings : SettingMetaSource.CustomSettings));
        var sourceValues = 0;
        var translatedValues = 0;
        Localizer.Initialize("zh-CN");
        foreach (var entry in settings)
        {
            var id = Convert.ToUInt32(((string)entry.Attribute("id")).Substring(2), 16);
            var file = (string)entry.Attribute("source");
            var source = files[file].Settings.Single(x => x.SettingId == id);
            Check((string)entry.Attribute("type") == "Dword", "Unexpected catalog value type");
            Check((string)entry.Element("Name").Element("Original") == source.UserfriendlyName, "Name source drift: " + id);
            Check(ParameterLocalizer.Name(id, source.UserfriendlyName) == (string)entry.Element("Name").Element("Translated"), "Chinese name: " + id);
            Check(ParameterLocalizer.Name(id, source.UserfriendlyName + " changed") == source.UserfriendlyName + " changed", "Changed source name fallback");
            if (entry.Element("Description") != null)
            {
                var original = (string)entry.Element("Description").Element("Original");
                var translated = (string)entry.Element("Description").Element("Translated");
                Check(original.Replace("\n", "") == source.Description.Replace("\n", ""), "Description source drift");
                Check(Tokens(original) == Tokens(translated), "Description token mismatch");
                var expected = DlssHelper.ReplaceDlssVersions(translated + "\r\n\r\n" + original).Replace("\\r\\n", "\r\n");
                Check(ParameterLocalizer.Description(id, source.Description).Replace("\r\n", "\n") == expected.Replace("\r\n", "\n"), "Bilingual description");
                Check(ParameterLocalizer.Description(id, "external description") == "external description", "Changed description fallback");
            }
            else Check(string.IsNullOrEmpty(source.Description), "Untranslated source description");
            var entries = entry.Element("Values").Elements("Value").ToList();
            Check(entries.Select(x => (string)x.Attribute("raw")).Distinct().Count() == entries.Count, "Duplicate value translation");
            var valueTranslations = entries.ToDictionary(x => Convert.ToUInt32(((string)x.Attribute("raw")).Substring(2), 16));
            var actualValues = metadata[file].GetDwordValues(id);
            foreach (var value in source.SettingValues)
            {
                sourceValues++;
                var raw = DrsUtil.GetDwordString(value.SettingValue);
                var actual = actualValues.Single(x => x.Value == value.SettingValue);
                var result = ParameterLocalizer.ValueName(id, "Dword", raw, actual.ValueName);
                var prefix = file == "Reference.xml" && !value.UserfriendlyName.StartsWith(raw) ? raw + " " : "";
                string expected;
                if (valueTranslations.TryGetValue(value.SettingValue, out var translation))
                {
                    translatedValues++;
                    Check((string)translation.Attribute("original") == value.UserfriendlyName, "Value label source drift");
                    Check(Tokens(value.UserfriendlyName) == Tokens((string)translation.Attribute("translated")), "Value token mismatch");
                    expected = prefix + DlssHelper.ReplaceDlssVersions((string)translation.Attribute("translated"));
                    Check(result == expected, "Translated value: " + actual.ValueName);
                    Check(ParameterLocalizer.ValueName(id, "Dword", raw, actual.ValueName + " edited") == actual.ValueName + " edited", "External value override fallback");
                    Check(ParameterLocalizer.ValueName(id, "Qword", raw, actual.ValueName) == actual.ValueName, "Type isolation");
                }
                else
                {
                    Check(Regex.IsMatch(value.UserfriendlyName, @"^(?:N/A|DLAA|MIN|MAX|[+-]?\d+(?:\.\d+)?(?: FPS|x|%)?|0x[0-9A-Fa-f]+)$"), "Missing human-readable value translation: " + value.UserfriendlyName);
                    expected = actual.ValueName;
                    Check(result == actual.ValueName, "Numeric/technical label changed");
                }
                // A translated dropdown item must retain the exact input accepted by DrsUtil.
                var row = Row(id, source.UserfriendlyName, actual.ValueName, source.GroupName, actualValues);
                var choice = row.ValueNameItems.Single(x => x.ValueName == actual.ValueName);
                Check(choice.DisplayName == expected && choice.ToString() == actual.ValueName, "Display/commit separation");
                Check(row.ValueDisplay == expected && row.SelectedValue == actual.ValueName && !row.IsModified, "Presentation must not dirty values");
                Check(choice.MatchesSearch(choice.DisplayName) && choice.MatchesSearch(actual.ValueName), "Bilingual dropdown search");
                var meta = new SettingMeta { DwordValues = actualValues };
                Check(DrsUtil.TryParseDwordSettingValue(meta, choice.ToString(), out var parsed) && parsed == value.SettingValue, "Translated choice parses to the original numeric value");
            }
            Localizer.Initialize("en");
            Check(ParameterLocalizer.Name(id, source.UserfriendlyName) == source.UserfriendlyName, "English name unchanged");
            foreach (var value in actualValues)
                Check(ParameterLocalizer.ValueName(id, "Dword", DrsUtil.GetDwordString(value.Value), value.ValueName) == value.ValueName, "English value unchanged");
            Check(ParameterLocalizer.Description(id, source.Description) == DlssHelper.ReplaceDlssVersions(source.Description ?? "").Replace("\\r\\n", "\r\n"), "English description unchanged");
            Localizer.Initialize("zh-CN");
        }
        Check(sourceValues == 1965 && translatedValues == 181, "Coverage changed; review the translation scope");
        Check(ParameterLocalizer.Name(0xDEADBEEF, "Unknown") == "Unknown", "Unknown setting fallback");
        Check(ParameterLocalizer.ValueName(0x1057EB71, "Dword", "0x12345678", "Unknown") == "Unknown", "Unknown value fallback");
        Check(ParameterLocalizer.ValueName(0x1057EB71, "String", "Adaptive", "Adaptive") == "Adaptive", "String value fallback");
        Check(ParameterLocalizer.ValueName(0x1057EB71, "Binary", "00", "Adaptive") == "Adaptive", "Binary value fallback");
        Check(ParameterLocalizer.ValueName(0x1057EB71, "Dword", "0x00000000", "0x00000000 Adaptive") == "0x00000000 自适应", "Optional raw prefix retained");
        GroupsAndRefresh(catalog);
        SearchMustPreserveEdits();
        return _checks;
    }

    private static void SearchMustPreserveEdits()
    {
        var values = new List<SettingValue<uint>>
        {
            new SettingValue<uint>(SettingMetaSource.CustomSettings) { Value = 0, ValueName = "Adaptive" },
            new SettingValue<uint>(SettingMetaSource.CustomSettings) { Value = 1, ValueName = "Prefer maximum performance" }
        };
        var row = Row(0x1057EB71, "Power Management - Mode", "Adaptive", "07 - System, Memory and Compute", values);
        var editor = new SearchableComboBox { IsEditable = true, DeferSelectionUntilCommit = true, IsTextSearchEnabled = false };
        editor.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(row.ValueNameItems)) { Source = row });
        editor.SetBinding(ComboBox.TextProperty, new Binding(nameof(row.SelectedValue))
        {
            Source = row, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        editor.SelectedItem = row.ValueNameItems[1];
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        Check(row.SelectedValue == "Prefer maximum performance" && row.IsModified, "Editor selection commits original English text");
        editor.FilterText = "自适应";
        Check(editor.Items.Count == 1 && editor.SelectedItem == null, "Filter hides selected item");
        Check(editor.Text == "Prefer maximum performance" && row.SelectedValue == "Prefer maximum performance" && row.IsModified, "Filtering must preserve unsaved editor text");
        editor.FilterText = "no match";
        Check(editor.Items.Count == 0 && row.IsModified && row.SelectedValue == "Prefer maximum performance", "Empty search results retain edits");
        editor.FilterText = "";
        Check(editor.Items.Count == 2 && row.SelectedValue == "Prefer maximum performance", "Clearing search retains edits");
        Check(BindingOperations.IsDataBound(editor, ItemsControl.ItemsSourceProperty), "Search retains the shared editor source binding");
        editor.SelectedItem = row.ValueNameItems[0];
        Check(row.SelectedValue == "Adaptive" && !row.IsModified, "Selecting original value resets modification flag");
        var manualText = "0x00000001";
        editor.SetCurrentValue(ComboBox.TextProperty, manualText);
        editor.FilterText = "最高性能";
        Check(editor.Text == manualText && row.SelectedValue == manualText, "Search retains manual numeric input");
    }

    private static string Tokens(string text) => string.Join("|", Regex.Matches(text, @"\$\{[^}]+\}").Cast<Match>().Select(x => x.Value).OrderBy(x => x));

    private static SettingItemViewModel Row(uint id, string name, string value, string group, List<SettingValue<uint>> values) => new SettingItemViewModel(new SettingItem
    {
        SettingId = id, SettingText = name, ValueText = value, GroupName = group, State = SettingState.NotAssiged
    }) { DwordValues = values };

    private static void GroupsAndRefresh(XDocument catalog)
    {
        var groups = catalog.Root.Element("Groups").Elements("Group").ToList();
        Check(groups.Count == 11 && groups.Select(x => (string)x.Attribute("original")).Distinct().Count() == 11, "Group coverage/duplicates");
        foreach (var group in groups)
        {
            var key = (string)group.Attribute("original");
            var header = new SettingGroupHeaderViewModel(key, false, _ => { });
            Check(header.Name == key && header.DisplayName == (string)group.Attribute("translated") && !header.IsExpanded, "Group display must not change its key/state");
        }
        var values = new List<SettingValue<uint>> { new SettingValue<uint>(SettingMetaSource.CustomSettings) { Value = 0, ValueName = "Adaptive" }, new SettingValue<uint>(SettingMetaSource.CustomSettings) { Value = 1, ValueName = "Prefer maximum performance" } };
        var row = Row(0x1057EB71, "Power Management - Mode", "Adaptive", "07 - System, Memory and Compute", values);
        Check(row.ValueDisplay == "自适应" && row.DisplayName == "电源管理 - 模式", "Initial presentation");
        Check(row.NameToolTip.Contains("Power Management - Mode") && row.NameToolTip.Contains("0x1057EB71"), "English name/ID hint");
        Check(SettingSearch.Matches(row.SettingId, row.DisplayName, null, row.SettingIdHex, "power management", row.SettingText), "English name search retained");
        SettingItemViewModel.ShowSettingIdInName = true;
        row.RefreshDisplayName();
        Check(row.OriginalDisplayName == "0x1057EB71 Power Management - Mode" && row.DisplayName == "0x1057EB71 电源管理 - 模式", "Keep the original decorated sort key");
        SettingItemViewModel.ShowSettingIdInName = false;
        row.RefreshDisplayName();
        row.SelectedValue = "Prefer maximum performance";
        Check(row.ValueDisplay == "最高性能优先" && row.IsModified, "Current value presentation cache refresh");
        row.UpdateValueSources(new List<SettingValue<uint>> { new SettingValue<uint>(SettingMetaSource.DriverSettings) { Value = 1, ValueName = "External label" } }, null, null, null);
        Check(row.ValueDisplay == "Prefer maximum performance" && row.IsModified, "Source refresh preserves edits and invalidates cached translations");
        row.IsFavorite = true;
        Check(row.GroupNameForDisplay == ParameterLocalizer.FavoritesKey && row.IsModified, "Favorite key stays canonical");
        Check(ParameterLocalizer.NormalizeGroupKey("⭐ 收藏") == ParameterLocalizer.FavoritesKey, "First-stage favorite key migration");
        var preferences = UserSettings.LoadSettings();
        var groupKey = "07 - System, Memory and Compute";
        preferences.HiddenSettingGroups = new List<string> { groupKey, "⭐ 收藏" };
        preferences.SaveSettings();
        var main = new MainViewModel(null, null, null, null);
        main.Settings.Clear();
        var groupedRow = Row(0x1057EB71, "Power Management - Mode", "Adaptive", groupKey, values);
        main.Settings.Add(groupedRow);
        main.FilterText = " ";
        var persistedHeader = main.GroupedSettingsView.OfType<SettingGroupHeaderViewModel>().Single();
        Check(persistedHeader.Name == groupKey && !persistedHeader.IsExpanded && main.GroupedSettingsView.Count == 1, "Persisted raw group key still collapses translated group");
        persistedHeader.IsExpanded = true;
        Check(!UserSettings.LoadSettings().HiddenSettingGroups.Contains(groupKey) && main.GroupedSettingsView.Count == 2, "Expanding translated header persists original key");
        main.GroupedSettingsView.OfType<SettingGroupHeaderViewModel>().Single().IsExpanded = false;
        Check(UserSettings.LoadSettings().HiddenSettingGroups.Contains(groupKey) && !UserSettings.LoadSettings().HiddenSettingGroups.Contains("07 - 系统、内存与计算"), "Collapsing translated header never persists display text");
        groupedRow.IsFavorite = true;
        main.FilterText = "  ";
        var favorites = main.GroupedSettingsView.OfType<SettingGroupHeaderViewModel>().Single();
        Check(!favorites.IsExpanded && favorites.Name == ParameterLocalizer.FavoritesKey, "Legacy Chinese favorite key remains collapsed");
        favorites.IsExpanded = true;
        Check(!UserSettings.LoadSettings().HiddenSettingGroups.Contains("⭐ 收藏"), "Legacy favorite key removed on expansion");
        row.SelectedValue = "0xFFFFFFFF";
        Check(row.ValueDisplay == "0xFFFFFFFF", "Manual numeric input remains raw");
        Localizer.Initialize("en");
        var english = Row(0x1057EB71, "Power Management - Mode", "Adaptive", "07 - System, Memory and Compute", values);
        Check(english.DisplayName == english.SettingText && english.ValueDisplay == "Adaptive", "New English session unchanged");
        Localizer.Initialize("zh-CN");
    }
}
