using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Wyvern.Utils.Validators
{
    public static class UsernameValidator
    {
        private static readonly string[] FilterUrls = new[]
        {
            "https://raw.githubusercontent.com/wyvern-social/filters/refs/heads/main/profanity.txt",
            "https://raw.githubusercontent.com/wyvern-social/filters/refs/heads/main/staff.txt",
        };

        private static readonly HttpClient Http = new HttpClient();
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        private static HashSet<string> _blockedCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static DateTimeOffset _blockedCacheExpires = DateTimeOffset.MinValue;

        public static async Task<ValidationResult> CheckAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return ValidationResult.Fail("Username cannot be empty.");

            if (username.Length < 2)
                return ValidationResult.Fail("Username must be at least 2 characters long.");

            if (username.Length > 15)
                return ValidationResult.Fail("Username may not be longer than 15 characters.");

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9._]+$"))
                return ValidationResult.Fail("Username can only contain letters, numbers, dots, or underscores.");

            if (Regex.IsMatch(username, @"[._]{2,}"))
                return ValidationResult.Fail("Username cannot contain consecutive dots or underscores.");

            if (username.IndexOf("wyvern", StringComparison.OrdinalIgnoreCase) >= 0)
                return ValidationResult.Fail("Username cannot contain 'wyvern'.");

            var blocked = await GetBlockedWordsAsync();
            var name = username.Trim();

            foreach (var phrase in blocked)
            {
                if (phrase.Length == 0) continue;
                if (name.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                    return ValidationResult.Fail("Username contains a blocked phrase.");
            }

            return ValidationResult.Ok();
        }

        private static async Task<HashSet<string>> GetBlockedWordsAsync()
        {
            var now = DateTimeOffset.UtcNow;
            if (now < _blockedCacheExpires && _blockedCache.Count > 0)
                return _blockedCache;

            var downloads = FilterUrls.Select(async url =>
            {
                try
                {
                    var text = await Http.GetStringAsync(url);
                    return text.Split('\n')
                               .Select(l => l.Trim())
                               .Where(l =>
                                    !string.IsNullOrEmpty(l) &&
                                    !l.StartsWith("#") &&
                                    !l.StartsWith("//") &&
                                    !l.StartsWith("/*"))
                               .ToArray();
                }
                catch
                {
                    return Array.Empty<string>();
                }
            });

            var lines = (await Task.WhenAll(downloads)).SelectMany(x => x);

            var set = new HashSet<string>(lines.Select(s => s.ToLowerInvariant()),
                                          StringComparer.OrdinalIgnoreCase);

            _blockedCache = set;
            _blockedCacheExpires = now.Add(CacheTtl);

            return _blockedCache;
        }
    }
}
