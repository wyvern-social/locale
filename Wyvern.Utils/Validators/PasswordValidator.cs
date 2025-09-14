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
                    "API.Validators.Password.Empty"
                ));
            }

            if (password.Length < 8)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "API.Validators.Password.TooShort"
                ));
            }

            if (password.Length > 64)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "API.Validators.Password.TooLong"
                ));
            }

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => char.IsPunctuation(ch) || char.IsSymbol(ch));

            if (!hasUpper)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "API.Validators.Password.MissingUpper"
                ));
            }

            if (!hasLower)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "API.Validators.Password.MissingLower"
                ));
            }

            if (!hasDigit)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "API.Validators.Password.MissingDigit"
                ));
            }

            if (!hasSpecial)
            {
                return Task.FromResult(ValidationResult.Fail(
                    "API.Validators.Password.MissingSpecial"
                ));
            }

            return Task.FromResult(ValidationResult.Ok());
        }
    }
}
