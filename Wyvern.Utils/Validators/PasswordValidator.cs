using System;
using System.Linq;
using System.Threading.Tasks;

namespace Wyvern.Utils.Validators
{
    public static class PasswordValidator
    {
        public static Task<ValidationResult> CheckAsync(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.empty",
                    "Password cannot be empty."
                ));
            }

            if (password.Length < 8)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.too_short",
                    "Password must be at least 8 characters."
                ));
            }

            if (password.Length > 64)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.too_long",
                    "Password must be at most 64 characters."
                ));
            }

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => char.IsPunctuation(ch) || char.IsSymbol(ch));

            if (!hasUpper)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.missing_upper",
                    "Password must contain at least one uppercase letter."
                ));
            }

            if (!hasLower)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.missing_lower",
                    "Password must contain at least one lowercase letter."
                ));
            }

            if (!hasDigit)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.missing_digit",
                    "Password must contain at least one number."
                ));
            }

            if (!hasSpecial)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "password.missing_special",
                    "Password must contain at least one special character."
                ));
            }

            return Task.FromResult(ValidationResult.Ok());
        }
    }
}
