using System;
using System.Globalization;
using System.Resources;
using System.Windows.Markup;

namespace nvidiaProfileInspector.Localization
{
    public static class Localizer
    {
        private static readonly ResourceManager Resources = new ResourceManager(
            "nvidiaProfileInspector.Localization.Strings", typeof(Localizer).Assembly);
        private static CultureInfo _culture = CultureInfo.GetCultureInfo("zh-CN");

        public static string Language => _culture.Name;

        public static string NormalizeLanguage(string language) =>
            string.Equals(language, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "zh-CN";

        // Use an explicit resource culture, including on the splash thread. Do not change
        // CurrentCulture: driver values and existing file formats must retain their semantics.
        public static void Initialize(string language)
        {
            _culture = CultureInfo.GetCultureInfo(NormalizeLanguage(language));
        }

        public static string Get(string key) => Resources.GetString(key, _culture) ?? key;

        public static string Format(string key, params object[] args) =>
            string.Format(CultureInfo.CurrentCulture, Get(key), args);
    }

    [MarkupExtensionReturnType(typeof(string))]
    public sealed class TextExtension : MarkupExtension
    {
        public TextExtension() { }
        public TextExtension(string key) { Key = key; }
        [ConstructorArgument("key")]
        public string Key { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) => Localizer.Get(Key);
    }
}
