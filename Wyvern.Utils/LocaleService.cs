using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Wyvern.API;

public class LocaleService
{
    private readonly Dictionary<string, JsonElement> _locales;

    public LocaleService(string resourcesPath)
    {
        var localesRoot = Path.Combine(resourcesPath, "Locales");
        _locales = LoadLocales(localesRoot);
    }

    private static Dictionary<string, JsonElement> LoadLocales(string localesRoot)
    {
        if (!Directory.Exists(localesRoot))
            return new Dictionary<string, JsonElement>();

        var dict = new Dictionary<string, JsonElement>();
        foreach (var file in Directory.GetFiles(localesRoot, "*.json"))
        {
            var key = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
            var json = JsonDocument.Parse(File.ReadAllText(file)).RootElement;
            dict[key] = json;
        }
        return dict;
    }

    public string GetString(string keyPath, string locale = "en_us")
    {
        locale = locale.ToLowerInvariant();
        if (!_locales.ContainsKey(locale))
            locale = "en_us";

        var jsonLocale = _locales[locale];
        return GetJsonValue(jsonLocale, keyPath) ?? keyPath;
    }

    public bool TryGetString(string keyPath, out string value, string locale = "en_us")
    {
        value = GetString(keyPath, locale);
        return value != keyPath;
    }

    public IEnumerable<string> GetAvailableLocales() => _locales.Keys;

    private static string? GetJsonValue(JsonElement element, string keyPath)
    {
        var parts = keyPath.Split('.');
        JsonElement current = element;

        foreach (var part in parts)
        {
            if (current.TryGetProperty(part, out var child))
                current = child;
            else
                return null;
        }

        return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
    }
}
