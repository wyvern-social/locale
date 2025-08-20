namespace Wyvern.Utils.Validators
{
    public class ValidationResult
    {
        public bool Success { get; }
        public string? ErrorMessage { get; }

        private ValidationResult(bool success, string? errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Ok()
        {
            return new ValidationResult(true, null);
        }

        public static ValidationResult Fail(string message)
        {
            return new ValidationResult(false, message);
        }

        public override string ToString()
        {
            return Success ? "Ok" : $"Error: {ErrorMessage}";
        }
    }
}
