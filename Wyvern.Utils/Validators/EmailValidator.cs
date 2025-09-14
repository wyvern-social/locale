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
                    "API.Validators.Email.Empty"
                );
            }

            if (email.Length > 320)
            {
                return ValidationResult.Fail(
                    "API.Validators.Email.TooLong"
                );
            }

            if (!EmailRegex.IsMatch(email))
            {
                return ValidationResult.Fail(
                    "API.Validators.Email.InvalidFormat"
                );
            }

            var parts = email.Split('@');
            if (parts.Length != 2)
            {
                return ValidationResult.Fail(
                    "API.Validators.Email.InvalidSymbol"
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
                    "API.Validators.Email.UnresolvableDomain",
                    "domain", domain
                );
            }

            return ValidationResult.Ok();
        }
    }
}
