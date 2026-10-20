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
        Check(settings.Count == 119, "Expected all CustomSettingNames plus two existing Reference translations");
        Check(settings.Select(x => (string)x.Attribute("id")).Distinct().Count() == 119, "Duplicate setting translation ID");
        var files = new[] { "CustomSettingNames.xml", "Reference.xml" }.ToDictionary(x => x,
            x => CustomSettingNames.FactoryLoadFromFile(Path.Combine(root, x)));
        var metadata = files.ToDictionary(x => x.Key, x => new CustomSettingMetaService(x.Value,
            x.Key == "Reference.xml" ? SettingMetaSource.ReferenceSettings : SettingMetaSource.CustomSettings));
        Check(files["CustomSettingNames.xml"].Settings.All(x => settings.Any(e => (string)e.Attribute("source") == "CustomSettingNames.xml" && Convert.ToUInt32(((string)e.Attribute("id")).Substring(2), 16) == x.SettingId)), "Every custom setting must be translated");
        var sourceValues = 0;
        var translatedValues = 0;
        Localizer.Initialize("zh-CN");
        foreach (var entry in settings)
        {
            var id = Convert.ToUInt32(((string)entry.Attribute("id")).Substring(2), 16);
            var file = (string)entry.Attribute("source");
            var source = files[file].Settings.Single(x => x.SettingId == id);
            var type = source.FallbackType == "QWORD" ? "Qword" : source.FallbackType == "STRING" ? "String" : "Dword";
            Check((string)entry.Attribute("type") == type, "Unexpected catalog value type");
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
            Check(Tokens(source.UserfriendlyName) == Tokens((string)entry.Element("Name").Element("Translated")), "Name token mismatch");
            Check(SettingSearch.Matches(id, ParameterLocalizer.Name(id, source.UserfriendlyName), null, DrsUtil.GetDwordString(id), (string)entry.Element("Name").Element("Translated"), source.UserfriendlyName), "Every translated name is searchable");
            Check(SettingSearch.Matches(id, ParameterLocalizer.Name(id, source.UserfriendlyName), null, DrsUtil.GetDwordString(id), source.UserfriendlyName, source.UserfriendlyName), "Every English name remains searchable");
            var entries = entry.Element("Values").Elements("Value").ToList();
            Check(entries.Select(x => (string)x.Attribute("raw")).Distinct().Count() == entries.Count, "Duplicate value translation");
            var valueTranslations = entries.ToDictionary(x => Convert.ToUInt64(((string)x.Attribute("raw")).Substring(2), 16));
            var dwords = type == "Dword" ? metadata[file].GetDwordValues(id) : null;
            var qwords = type == "Qword" ? metadata[file].GetQwordValues(id) : null;
            Check(type != "String" || source.SettingValues.Count == 0, "Review string option coverage if metadata adds options");
            foreach (var translation in entries)
                Check(source.SettingValues.Any(v => v.QwordSettingValue == Convert.ToUInt64(((string)translation.Attribute("raw")).Substring(2), 16) && v.UserfriendlyName == (string)translation.Attribute("original")), "Translation references an absent original value");
            for (var index = 0; index < source.SettingValues.Count; index++)
            {
                var value = source.SettingValues[index];
                sourceValues++;
                var number = value.QwordSettingValue;
                var raw = type == "Qword" ? DrsUtil.GetQwordString(number) : DrsUtil.GetDwordString((uint)number);
                var original = type == "Qword" ? qwords[index].ValueName : dwords[index].ValueName;
                var result = ParameterLocalizer.ValueName(id, type, raw, original);
                var prefix = file == "Reference.xml" && !value.UserfriendlyName.StartsWith(raw) ? raw + " " : "";
                string expected;
                if (valueTranslations.TryGetValue(number, out var translation) && (string)translation.Attribute("original") == value.UserfriendlyName)
                {
                    translatedValues++;
                    Check(Tokens(value.UserfriendlyName) == Tokens((string)translation.Attribute("translated")), "Value token mismatch");
                    expected = prefix + DlssHelper.ReplaceDlssVersions((string)translation.Attribute("translated"));
                    Check(result == expected, "Translated value: " + original);
                    Check(ParameterLocalizer.ValueName(id, type, raw, original + " edited") == original + " edited", "External value override fallback");
                    Check(ParameterLocalizer.ValueName(id, type == "Qword" ? "Dword" : "Qword", raw, original) == original, "Type isolation");
                }
                else
                {
                    Check(Regex.IsMatch(value.UserfriendlyName, @"^(?:N/A|DLAA|MIN|MAX|[+-]?\d+(?:\.\d+)?(?: FPS|x|%|MB|GB)?|0x[0-9A-Fa-f]+)$"), "Missing human-readable value translation: " + value.UserfriendlyName);
                    expected = original;
                    Check(result == original, "Numeric/technical label changed");
                }
                // Match by original label and position: upstream can assign multiple labels to the same raw value.
                var row = new SettingItemViewModel(new SettingItem
                {
                    SettingId = id, SettingText = source.UserfriendlyName, ValueText = original,
                    GroupName = source.GroupName, State = SettingState.NotAssiged
                }) { DwordValues = dwords, QwordValues = qwords };
                var choice = row.ValueNameItems.First(x => x.ValueName == original);
                Check(choice.DisplayName == expected && choice.ToString() == original, "Display/commit separation");
                Check(row.ValueDisplay == expected && row.SelectedValue == original && !row.IsModified, "Presentation must not dirty values");
                Check(choice.MatchesSearch(choice.DisplayName) && choice.MatchesSearch(original), "Bilingual dropdown search");
                var meta = new SettingMeta { DwordValues = dwords, QwordValues = qwords };
                if (type == "Qword")
                    Check(DrsUtil.TryParseQwordSettingValue(meta, choice.ToString(), out var parsed) && parsed == qwords.First(x => x.ValueName == original).Value, "QWORD choice preserves existing English parsing");
                else
                    Check(DrsUtil.TryParseDwordSettingValue(meta, choice.ToString(), out var parsed) && parsed == dwords.First(x => x.ValueName == original).Value, "DWORD choice preserves existing English parsing");
                if (type == "Qword")
                    Check(DrsUtil.TryParseQwordSettingValue(meta, raw, out var rawParsed) && rawParsed == number, "Manual QWORD retains full raw value");
                else
                    Check(DrsUtil.TryParseDwordSettingValue(meta, raw, out var rawParsed) && rawParsed == number, "Manual DWORD retains full raw value");
                Localizer.Initialize("en");
                Check(ParameterLocalizer.ValueName(id, type, raw, original) == original, "English value unchanged");
                Localizer.Initialize("zh-CN");
            }
            Localizer.Initialize("en");
            Check(ParameterLocalizer.Name(id, source.UserfriendlyName) == source.UserfriendlyName, "English name unchanged");
            Check(ParameterLocalizer.Description(id, source.Description) == DlssHelper.ReplaceDlssVersions(source.Description ?? "").Replace("\\r\\n", "\r\n"), "English description unchanged");
            Localizer.Initialize("zh-CN");
        }
        Check(sourceValues == 2422 && translatedValues == 363, "Coverage changed; review the translation scope");
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
