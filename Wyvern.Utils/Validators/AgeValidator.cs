using System;

namespace Wyvern.Utils.Validators
{
    public static class AgeValidator
    {
        public static ValidationResult Check(DateTime birthday)
        {
            var today = DateTime.UtcNow.Date;

            var age = today.Year - birthday.Year;
            if (today.Month < birthday.Month ||
                (today.Month == birthday.Month && today.Day < birthday.Day))
            {
                age--;
            }

            if (age < 0)
            {
                return ValidationResult.Fail("Birthday cannot be in the future.");
            }

            if (age < 13)
            {
                return ValidationResult.Fail("You must be at least 13 years old to use this service.");
            }

            if (age > 100)
            {
                return ValidationResult.Fail($"Are you really {age} years old? If so, please contact support — we’ll buy you a cake!");
            }

            return ValidationResult.Ok();
        }
    }
}
