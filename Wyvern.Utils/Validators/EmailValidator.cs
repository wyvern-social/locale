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
                return ValidationResult.Fail("Email cannot be empty.");

            if (email.Length > 320)
                return ValidationResult.Fail("Email must be at most 320 characters.");

            if (!EmailRegex.IsMatch(email))
                return ValidationResult.Fail("Email format is invalid.");

            var parts = email.Split('@');
            if (parts.Length != 2)
                return ValidationResult.Fail("Email must contain a single '@' symbol.");

            var domain = parts[1];

            try
            {
                await Dns.GetHostAddressesAsync(domain);
            }
            catch
            {
                return ValidationResult.Fail($"Domain '{domain}' could not be resolved.");
            }

            return ValidationResult.Ok();
        }
    }
}
