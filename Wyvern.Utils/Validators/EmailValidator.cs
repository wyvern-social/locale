using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wyvern.Utils.Validators
{
    public static class EmailValidator
    {
        private static readonly Regex EmailRegex = new(
            @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public static async Task<ValidationResult> CheckAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Fail(
                    "email.empty",
                    "Email cannot be empty."
                );
            }

            if (email.Length > 320)
            {
                return ValidationResult.Fail(
                    "email.too_long",
                    "Email must be at most 320 characters."
                );
            }

            if (!EmailRegex.IsMatch(email))
            {
                return ValidationResult.Fail(
                    "email.invalid_format",
                    "Email format is invalid."
                );
            }

            var parts = email.Split('@');
            if (parts.Length != 2)
            {
                return ValidationResult.Fail(
                    "email.invalid_symbol",
                    "Email must contain a single '@' symbol."
                );
            }

            var domain = parts[1];

            try
            {
                await Dns.GetHostAddressesAsync(domain);
            }
            catch
            {
                return ValidationResult.Fail(
                    "email.unresolvable_domain",
                    $"Domain '{domain}' could not be resolved."
                );
            }

            return ValidationResult.Ok();
        }
    }
}
