using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using nvidiaProfileInspector.Common.Helper;

namespace nvidiaProfileInspector.Localization
{
    /// <summary>Display-only translations. Driver metadata and editable value text stay original.</summary>
    public static class ParameterLocalizer
    {
        public const string FavoritesKey = "⭐ Favorites";
        private const string ResourceName = "nvidiaProfileInspector.Localization.ParameterTranslations.zh-CN.xml";
        private static readonly Lazy<Catalog> Translations = new Lazy<Catalog>(Load);

        private sealed class Field
        {
            public string Original;
            public string Translated;
        }

        private sealed class Setting
        {
            public Field Name;
            public Field Description;
            public readonly Dictionary<string, Field> Values = new Dictionary<string, Field>(StringComparer.Ordinal);
        }

        private sealed class Catalog
        {
            public readonly Dictionary<uint, Setting> Settings = new Dictionary<uint, Setting>();
            public readonly Dictionary<string, string> Groups = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        private static bool IsChinese => Localizer.Language == "zh-CN";

        public static string Name(uint id, string original)
        {
            if (IsChinese && Translations.Value.Settings.TryGetValue(id, out var setting))
                return Translate(setting.Name, original);
            return original;
        }

        public static string Description(uint id, string original)
        {
            var text = original;
            if (IsChinese && Translations.Value.Settings.TryGetValue(id, out var setting))
            {
                var translated = Translate(setting.Description, original);
                if (!string.Equals(translated, original, StringComparison.Ordinal))
                    text = translated + "\r\n\r\n" + original;
            }
            // Description comparison happens before token expansion; both languages share
            // the same version resolver. Normalize both XML-style and real line endings.
            return DlssHelper.ReplaceDlssVersions(text ?? "").Replace("\\r\\n", "\r\n");
        }

        public static string GroupName(string original)
        {
            if (original == FavoritesKey) return Localizer.Get("Favorites_7f6d0c");
            return IsChinese && original != null && Translations.Value.Groups.TryGetValue(original, out var translated)
                ? translated : original;
        }

        // Only the first-stage Chinese favorites label ever served as a translated storage key.
        // Accept that old key when reading preferences; ordinary groups remain exact raw keys.
        public static string NormalizeGroupKey(string key) => key == "⭐ 收藏" ? FavoritesKey : key;

        public static string ValueName(uint id, string type, string raw, string original)
        {
            if (!IsChinese || original == null || !Translations.Value.Settings.TryGetValue(id, out var setting)
                || !setting.Values.TryGetValue(ValueKey(type, raw), out var field) || field.Translated == null) return original;

            // Metadata has already expanded DLSS tokens. Compare the same expanded original,
            // and tolerate only the known raw-value prefix added by the metadata service.
            var expected = DlssHelper.ReplaceDlssVersions(field.Original);
            var translated = DlssHelper.ReplaceDlssVersions(field.Translated);
            if (string.Equals(original, expected, StringComparison.Ordinal)) return translated;
            var prefix = raw + " ";
            return string.Equals(original, prefix + expected, StringComparison.Ordinal)
                ? prefix + translated : original;
        }

        private static string ValueKey(string type, string raw) => type + ":" +
            ((type == "Dword" || type == "Qword") ? raw?.ToUpperInvariant() : raw);

        private static string Translate(Field field, string original) => field?.Translated != null
            && string.Equals(NormalizeLines(field.Original), NormalizeLines(original), StringComparison.Ordinal)
                ? field.Translated : original;

        private static string NormalizeLines(string text) => text?.Replace("\\r\\n", "\n").Replace("\r\n", "\n");

        private static Field ReadField(XElement element) => element == null ? null : new Field
        {
            Original = (string)element.Element("Original"),
            Translated = (string)element.Element("Translated")
        };

        private static Catalog Load()
        {
            try
            {
                using var stream = typeof(ParameterLocalizer).Assembly.GetManifestResourceStream(ResourceName);
                if (stream == null) return new Catalog();
                var document = XDocument.Load(stream);
                var catalog = new Catalog();
                if (document.Root?.Element("Groups") == null || document.Root.Element("Settings") == null) return catalog;
                foreach (var group in document.Root.Element("Groups").Elements("Group"))
                    catalog.Groups.Add((string)group.Attribute("original"), (string)group.Attribute("translated"));
                foreach (var node in document.Root.Element("Settings").Elements("Setting"))
                {
                    var id = uint.Parse(((string)node.Attribute("id")).Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    var setting = new Setting { Name = ReadField(node.Element("Name")), Description = ReadField(node.Element("Description")) };
                    foreach (var value in node.Element("Values").Elements("Value"))
                        setting.Values.Add(ValueKey((string)node.Attribute("type"), (string)value.Attribute("raw")),
                            new Field { Original = (string)value.Attribute("original"), Translated = (string)value.Attribute("translated") });
                    catalog.Settings.Add(id, setting);
                }
                return catalog;
            }
            catch (Exception ex) when (ex is System.Xml.XmlException || ex is ArgumentException
                || ex is InvalidOperationException || ex is FormatException || ex is OverflowException)
            {
                // Translation data must never prevent editing driver profiles. Offline checks
                // validate the embedded catalog; a malformed catalog falls back to English.
                Debug.WriteLine("Parameter translation resource unavailable: " + ex.Message);
                return new Catalog();
            }
        }
    }
}
