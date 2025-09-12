namespace Wyvern.Utils.Validators
{
    public class ValidationResult
    {
        public bool Success { get; }
        public string? MessageKey { get; }
        public string? Message { get; }

        private ValidationResult(bool success, string? messageKey = null, string? message = null)
        {
            Success = success;
            MessageKey = messageKey;
            Message = message;
        }

        public static ValidationResult Ok()
            => new ValidationResult(true);

        public static ValidationResult Fail(string messageKey, string message)
            => new ValidationResult(false, messageKey, message);
    }
}
