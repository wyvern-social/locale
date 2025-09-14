using System.Collections.Generic;

namespace Wyvern.Utils.Validators
{
    public class ValidationResult
    {
        public bool Success { get; }
        public string? MessageKey { get; }
        public Dictionary<string, object>? Data { get; }

        private ValidationResult(bool success, string? messageKey = null, Dictionary<string, object>? data = null)
        {
            Success = success;
            MessageKey = messageKey;
            Data = data;
        }

        public static ValidationResult Ok()
            => new ValidationResult(true);

        public static ValidationResult Fail(string messageKey, Dictionary<string, object>? data = null)
            => new ValidationResult(false, messageKey, data);

        public static ValidationResult Fail(string messageKey, string key, object value)
            => new ValidationResult(false, messageKey, new Dictionary<string, object> { { key, value } });
    }
}
