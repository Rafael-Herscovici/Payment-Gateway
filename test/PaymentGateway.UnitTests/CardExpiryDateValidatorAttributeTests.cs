using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class CardExpiryDateValidatorAttributeTests
    {
        [Fact]
        public void IsValid_WithHappyPath_ShouldReturnTrue()
        {
            // Arrange
            var cardDetails = new CardDetails { CardExpiryDate = DateTime.UtcNow.ToString(DateTime.UtcNow.ToString(Constants.ExpiryDateFormat)) };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(cardDetails);

            // Act
            Validator.TryValidateObject(cardDetails, context, validationResults, true);

            // Assert
            Assert.DoesNotContain(validationResults, x => x.MemberNames.Contains(nameof(CardDetails.CardExpiryDate)));
        }

        [Fact]
        public void IsValid_WithInvalidDate_ShouldReturnFalse()
        {
            // Arrange
            var cardDetails = new CardDetails { CardExpiryDate = DateTime.UtcNow.ToString("01-01") };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(cardDetails);

            // Act
            Validator.TryValidateObject(cardDetails, context, validationResults, true);

            // Assert
            Assert.Contains(validationResults, x => x.MemberNames.Contains(nameof(CardDetails.CardExpiryDate)));
        }

        [Fact]
        public void IsValid_WithInvalidFormat_ShouldReturnFalse()
        {
            // Arrange
            var cardDetails = new CardDetails { CardExpiryDate = DateTime.UtcNow.ToString("01/01") };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(cardDetails);

            // Act
            Validator.TryValidateObject(cardDetails, context, validationResults, true);

            // Assert
            Assert.Contains(validationResults, x => x.MemberNames.Contains(nameof(CardDetails.CardExpiryDate)));
        }

        [Fact]
        public void IsValid_WithEmptyDate_ShouldReturnFalse()
        {
            // Arrange
            var cardDetails = new CardDetails();
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(cardDetails);

            // Act
            Validator.TryValidateObject(cardDetails, context, validationResults, true);

            // Assert
            Assert.Contains(validationResults, x => x.MemberNames.Contains(nameof(CardDetails.CardExpiryDate)));
        }
    }
}
