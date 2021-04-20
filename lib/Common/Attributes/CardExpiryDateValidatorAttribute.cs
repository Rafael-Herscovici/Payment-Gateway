using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Common.Attributes
{
    /// <summary>
    /// Validates the structure and "date in future" of the expiry date
    /// </summary>
    public class CardExpiryDateValidatorAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var expiryDate = value?.ToString();
            if (string.IsNullOrWhiteSpace(expiryDate))
                return new ValidationResult($"{validationContext.DisplayName} is required.", new List<string> { validationContext.DisplayName });

            var pattern = @"^(0[1-9]|1[0-2])\" + Constants.DateSeparator + "?(([0-9]{2})$)";
            if (!Regex.IsMatch(expiryDate, pattern))
                return new ValidationResult($"{validationContext.DisplayName} must be in {Constants.ExpiryDateFormat} format.", new List<string> { validationContext.DisplayName });

            if (DateTime.TryParseExact(expiryDate, Constants.ExpiryDateFormat, null, DateTimeStyles.None, out var result))
            {
                var now = DateTime.UtcNow;
                if (result.Year >= now.Month && result.Year >= now.Year)
                    return ValidationResult.Success;
            }

            return new ValidationResult($"{validationContext.DisplayName} is invalid.", new List<string> { validationContext.DisplayName });
        }
    }
}
