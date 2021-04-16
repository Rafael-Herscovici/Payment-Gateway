using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PaymentGatewayAPI.Attributes
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
            if (string.IsNullOrEmpty(expiryDate))
                return new ValidationResult(validationContext.DisplayName + " is required.");

            if (!Regex.IsMatch(expiryDate, @$"^(0[1-9]|1[0-2])\{Constants.DateSeparator}?(([0-9]{2})$)"))
                return new ValidationResult($"Must be in MM{Constants.DateSeparator}yy format.");

            var split = expiryDate.Split(Constants.DateSeparator);
            if (DateTime.TryParseExact($"{split[0]}{split[1]}", "MMyy", null, DateTimeStyles.None, out var result))
            {
                var now = DateTime.UtcNow;
                if (result.Year >= now.Month && result.Year >= now.Year)
                    return ValidationResult.Success;
            }

            return new ValidationResult("The expiry date is invalid.");
        }
    }
}
