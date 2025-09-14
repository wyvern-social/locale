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
                return ValidationResult.Fail(
                    "API.Validators.Age.InvalidBirthday"
                );
            }

            if (age < 13)
            {
                return ValidationResult.Fail(
                    "API.Validators.Age.TooYoung"
                );
            }

            if (age > 100)
            {
                return ValidationResult.Fail(
                    "API.Validators.Age.Suspicious",
                    "age", age
                );
            }

            return ValidationResult.Ok();
        }
    }
}
