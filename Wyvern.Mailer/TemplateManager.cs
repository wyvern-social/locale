using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using HandlebarsDotNet;

namespace Wyvern.Mailer;

public static class TemplateManager
{
    private static readonly string ResourcesDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Resources");
    private static readonly string LocalesRoot = Path.Combine(ResourcesDir, "Locales");
    private static readonly string TemplatesRoot = Path.Combine(ResourcesDir, "MailTemplates");
    private static readonly Dictionary<string, JsonElement> Locales = LoadLocales();

    static TemplateManager()
    {
        Handlebars.RegisterHelper("year", (writer, context, parameters) =>
        {
            writer.Write(DateTime.UtcNow.Year.ToString());
        });
    }

    private static Dictionary<string, JsonElement> LoadLocales()
    {
        if (!Directory.Exists(LocalesRoot))
            return new Dictionary<string, JsonElement>();

        var dict = new Dictionary<string, JsonElement>();
        foreach (var file in Directory.GetFiles(LocalesRoot, "*.json"))
        {
            var key = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
            var json = JsonDocument.Parse(File.ReadAllText(file)).RootElement;
            dict[key] = json;
        }
        return dict;
    }

    public static string RenderTemplate(string templateName, object data, string locale = "en_us")
    {
        locale = locale.ToLowerInvariant();
        if (!Locales.ContainsKey(locale))
            locale = "en_us";

        var jsonLocale = Locales[locale];
        var templatePath = Path.Combine(TemplatesRoot, templateName);
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template not found: {templatePath}");

        var source = File.ReadAllText(templatePath);
        var template = Handlebars.Compile(source);

        Handlebars.RegisterHelper("t", (writer, context, parameters) =>
        {
            if (parameters.Length == 0) return;
            var keyPath = parameters[0]?.ToString() ?? "";
            var value = GetJsonValue(jsonLocale, keyPath) ?? keyPath;

            var innerTemplate = Handlebars.Compile(value);
            writer.Write(innerTemplate(data));
        });

        return template(data);
    }

    public static string GetLocaleString(string keyPath, object? data = null, string locale = "en_us")
    {
        locale = locale.ToLowerInvariant();
        if (!Locales.ContainsKey(locale))
            locale = "en_us";

        var jsonLocale = Locales[locale];
        var value = GetJsonValue(jsonLocale, keyPath) ?? keyPath;

        if (data != null)
        {
            var innerTemplate = Handlebars.Compile(value);
            return innerTemplate(data);
        }

        return value;
    }

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

    public static IEnumerable<string> GetAvailableLocales() => Locales.Keys;

    public static string GetTemplateName(EmailType type) => type.ToString().ToLowerInvariant() + ".hbs";
    public static string GetSubjectKey(EmailType type) => type.ToString() + ".Subject";
}
