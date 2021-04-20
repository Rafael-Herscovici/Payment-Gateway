using AutoFixture;
using AutoMapper;
using Common;
using Common.Entities;
using Common.Enums;
using Common.Models;
using CurrencyExchange;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGatewayAPI.Models.Mappings;
using PaymentGatewayAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class DbAccessTests : UnitTestBase
    {
        private readonly Mock<ILogger<DbAccess>> _mockLogger =
            new Mock<ILogger<DbAccess>>();
        private readonly Mock<IBankAccess> _mockBankAccess =
            new Mock<IBankAccess>();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task ProcessPaymentAsync_WithHappyPath_ShouldCompleteSuccessfullyAsync()
        {
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            // Arrange
            CancellationToken cancellationToken = default;
            var paymentRequestModel = _fixture.Create<PaymentRequest>();
            _mockBankAccess.Setup(bankAccess =>
                    bankAccess.ProcessPaymentAsync(
                        paymentRequestModel,
                        cancellationToken))
                .ReturnsAsync(PaymentStatus.Success);

            var sut = new DbAccess(
                _mockLogger.Object,
                GetMapper(),
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext);

            // Act
            var result = await sut.ProcessPaymentAsync(paymentRequestModel, cancellationToken);

            // Assert
            Assert.Equal(PaymentStatus.Success, result.Status);
            Assert.NotEqual(Guid.Empty, result.PaymentId);
            _mockBankAccess.VerifyAll();
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenBankAccessFails_ShouldThrowExceptionAsync()
        {
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            // Arrange
            CancellationToken cancellationToken = default;
            var paymentRequestModel = _fixture.Create<PaymentRequest>();

            var expectedException = new Exception("Something went wrong...");
            _mockBankAccess.Setup(bankAccess =>
                    bankAccess.ProcessPaymentAsync(
                        paymentRequestModel,
                        cancellationToken))
                .ThrowsAsync(expectedException);

            var sut = new DbAccess(
                _mockLogger.Object,
                GetMapper(),
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.ProcessPaymentAsync(paymentRequestModel, cancellationToken));

            // Assert
            Assert.Equal(expectedException, ex);
            _mockBankAccess.VerifyAll();
        }

        [Fact]
        public async Task GetPaymentByIdAsync_WithHappyPath_ShouldCompleteSuccessfullyAsync()
        {
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            // Arrange
            CancellationToken cancellationToken = default;
            var paymentRequestModel = _fixture.Create<PaymentRequest>();
            // Manually update the expiry date
            paymentRequestModel.CardDetails.CardExpiryDate = DateTime.UtcNow.ToString($"MM{Constants.DateSeparator}yy");

            var sut = new DbAccess(
                _mockLogger.Object,
                GetMapper(),
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext);
            // Seed the entity
            var paymentResponse = await sut.ProcessPaymentAsync(paymentRequestModel, cancellationToken);

            // Act
            var result = await sut.GetPaymentByIdAsync(paymentResponse.PaymentId, cancellationToken);

            // Assert
            Assert.Equal(paymentResponse.Status, result.Status);
            Assert.Equal(paymentResponse.PaymentId, result.PaymentId);
            Assert.Equal(paymentRequestModel.MerchantId, result.MerchantId);
            Assert.Equal(paymentRequestModel.Currency, result.Currency);
            Assert.Equal(paymentRequestModel.Amount, result.Amount);
            Assert.StartsWith(new string(Constants.MaskCharacter, 12), result.CardDetails.CardNumber);
            Assert.Equal(new string(Constants.MaskCharacter, 3), result.CardDetails.CardSecurityCode);
            Assert.Equal($"{new string(Constants.MaskCharacter, 2)}{Constants.DateSeparator}{new string(Constants.MaskCharacter, 2)}", result.CardDetails.CardExpiryDate);
            _mockBankAccess.VerifyAll();
        }

        [Theory]
        [InlineData(Constants.DefaultCurrency, true)]
        [InlineData("XYZ", false)]
        public async Task IsValidCurrencyAsync_ShouldReturnBooleanValueAsync(string currency, bool expectedResult)
        {
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            // Arrange
            CancellationToken cancellationToken = default;
            await SeedCurrenciesDatabase();

            var sut = new DbAccess(
                _mockLogger.Object,
                GetMapper(),
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext);

            // Act
            var result = await sut.IsValidCurrencyAsync(currency, cancellationToken);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetSupportedCurrencies_ShouldReturnAListOfSupportedCurrenciesAsync()
        {
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            // Arrange
            CancellationToken cancellationToken = default;
            await SeedCurrenciesDatabase();

            var sut = new DbAccess(
                _mockLogger.Object,
                GetMapper(),
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext);

            // Act
            var result = await sut.GetSupportedCurrenciesAsync(cancellationToken);

            // Assert
            Assert.Single(result, currency => currency.Equals(Constants.DefaultCurrency));
        }


        private static IMapper GetMapper()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                {
                    "AesEncryptionKey", "FakeAesKey"
                } })
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<PaymentRequestEntityProfile.MaskCardDetailsResolver>();
            services.AddSingleton<PaymentRequestEntityProfile.EncryptCardDetailsResolver>();
            services.AddSingleton(new Encryption(configuration));
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<PaymentRequestEntityProfile>();
            });
            var provider = services.BuildServiceProvider();

            return provider.GetService<IMapper>();
        }

        private async Task SeedCurrenciesDatabase()
        {
            await using var context = new CurrencyExchangeDbContext(GetInMemoryDbContextOptions<CurrencyExchangeDbContext>());
            var defaultCurrency = new CurrencyEntity
            {
                Currency = Constants.DefaultCurrency,
                Rate = 1
            };
            if (!await context.Currencies.ContainsAsync(defaultCurrency))
                await context.Currencies.AddAsync(new CurrencyEntity
                {
                    Currency = Constants.DefaultCurrency,
                    Rate = 1
                });
            // Seed here
            await context.SaveChangesAsync();
        }
    }
}
