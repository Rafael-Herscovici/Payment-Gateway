using AutoFixture;
using AutoMapper;
using Common;
using Common.Models;
using CurrencyExchange;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGatewayAPI.Controllers;
using PaymentGatewayAPI.Models;
using PaymentGatewayAPI.Services;
using PaymentGatewayDB;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class PaymentControllerTests : UnitTestBase
    {
        private readonly Mock<ILogger<PaymentController>> _mockLogger =
            new Mock<ILogger<PaymentController>>();
        private readonly Mock<IMapper> _mockMapper =
            new Mock<IMapper>();
        private readonly Mock<IBankAccess> _mockBankAccess =
            new Mock<IBankAccess>();

        private readonly Fixture _fixture =
            new Fixture();

        [Fact]
        public async Task GetPaymentById_WithHappyPath_ShouldReturnOKPaymentHistoricAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            var expectedResult = _fixture.Create<PaymentHistoric>();

            var mockDbAccess = GetDbAccessMock(gatewayDbContext, exchangeDbContext);
            mockDbAccess.Setup(x =>
                    x.GetPaymentByIdAsync(
                        expectedResult.PaymentId,
                        cancellationToken))
                .ReturnsAsync(expectedResult);

            var sut = new PaymentController();

            // Act
            var actionResult = await sut.GetPaymentByIdAsync(mockDbAccess.Object, expectedResult.PaymentId, cancellationToken);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result);

            var paymentHistoric = result.Value as PaymentHistoric;
            Assert.NotNull(paymentHistoric);
            Assert.Equal(expectedResult, paymentHistoric);

            mockDbAccess.VerifyAll();
        }

        [Fact]
        public async Task GetPaymentById_WithInvalidId_ShouldReturnNotFoundAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();
            var mockDbAccess = new Mock<DbAccess>(
                new Mock<ILogger<DbAccess>>().Object,
                _mockMapper.Object,
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext
            );

            var paymentId = Guid.NewGuid();

            mockDbAccess.Setup(x =>
                    x.GetPaymentByIdAsync(
                        paymentId,
                        cancellationToken))
                .ReturnsAsync(() => null);

            var sut = new PaymentController();

            // Act
            var actionResult = await sut.GetPaymentByIdAsync(mockDbAccess.Object, paymentId, cancellationToken);

            // Assert
            var result = actionResult.Result as NotFoundObjectResult;
            Assert.NotNull(result);

            var problemDetails = result.Value as ProblemDetails;
            Assert.NotNull(problemDetails);
            Assert.Equal("The requested Payment id could not be found.", problemDetails.Detail);

            mockDbAccess.VerifyAll();
        }

        [Fact]
        public async Task ProcessPaymentAsync_WithHappyPath_ShouldReturnOKPaymentResponseAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            var paymentRequest = _fixture.Create<PaymentRequest>();
            var expectedPaymentResponse = _fixture.Create<PaymentResponse>();

            var mockDbAccess = GetDbAccessMock(gatewayDbContext, exchangeDbContext);
            mockDbAccess.Setup(x =>
                    x.IsValidCurrencyAsync(
                        paymentRequest.Currency,
                        cancellationToken))
                .ReturnsAsync(true);

            mockDbAccess.Setup(x =>
                    x.ProcessPaymentAsync(
                        paymentRequest,
                        cancellationToken))
                .ReturnsAsync(expectedPaymentResponse);

            var sut = new PaymentController();
            // Act
            var actionResult = await sut.ProcessPaymentAsync(
                _mockLogger.Object,
                mockDbAccess.Object,
                new TelemetryClient(new TelemetryConfiguration($"Test-{Guid.NewGuid()}")),
                paymentRequest,
                cancellationToken);

            // Assert
            var result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result);

            var paymentResponse = result.Value as PaymentResponse;
            Assert.NotNull(paymentResponse);
            Assert.Equal(expectedPaymentResponse, paymentResponse);
            mockDbAccess.VerifyAll();
        }

        [Fact]
        public async Task ProcessPaymentAsync_WithInvalidCurrencyAndModel_ShouldReturnBadRequestAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            var paymentRequest = new PaymentRequest();

            var mockDbAccess = GetDbAccessMock(gatewayDbContext, exchangeDbContext);
            mockDbAccess.Setup(x =>
                    x.IsValidCurrencyAsync(
                        paymentRequest.Currency,
                        cancellationToken))
                .ReturnsAsync(false);

            mockDbAccess.Setup(x =>
                    x.GetSupportedCurrenciesAsync(
                        cancellationToken))
                .ReturnsAsync(new List<string> { Constants.DefaultCurrency });

            var sut = new PaymentController();
            // Act
            var actionResult = await sut.ProcessPaymentAsync(
                _mockLogger.Object,
                mockDbAccess.Object,
                new TelemetryClient(new TelemetryConfiguration($"Test-{Guid.NewGuid()}")),
                paymentRequest,
                cancellationToken);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.NotNull(result);

            var validationProblem = result.Value as SerializableError;
            Assert.NotNull(validationProblem);
            Assert.True(validationProblem.ContainsKey("Currency"));
            mockDbAccess.VerifyAll();
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenDbAccessThrows_ShouldReturnBadRequestAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            await using var gatewayDbContext = GetGatewayDbContext();
            await using var exchangeDbContext = GetExchangeDbContext();

            var paymentRequest = new PaymentRequest();

            var mockDbAccess = GetDbAccessMock(gatewayDbContext, exchangeDbContext);
            mockDbAccess.Setup(x =>
                    x.IsValidCurrencyAsync(
                        paymentRequest.Currency,
                        cancellationToken))
                .ReturnsAsync(false);

            mockDbAccess.Setup(x =>
                    x.GetSupportedCurrenciesAsync(
                        cancellationToken))
                .ReturnsAsync(new List<string> { Constants.DefaultCurrency });

            var sut = new PaymentController();
            // Act
            var actionResult = await sut.ProcessPaymentAsync(
                _mockLogger.Object,
                mockDbAccess.Object,
                new TelemetryClient(new TelemetryConfiguration($"Test-{Guid.NewGuid()}")), 
                paymentRequest,
                cancellationToken);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.NotNull(result);

            var validationProblem = result.Value as SerializableError;
            Assert.NotNull(validationProblem);
            Assert.True(validationProblem.ContainsKey("Currency"));
            mockDbAccess.VerifyAll();
        }

        private Mock<DbAccess> GetDbAccessMock(
            PaymentGatewayDbContext gatewayDbContext,
            CurrencyExchangeDbContext exchangeDbContext)
        {
            return new Mock<DbAccess>(
                new Mock<ILogger<DbAccess>>().Object,
                _mockMapper.Object,
                _mockBankAccess.Object,
                gatewayDbContext,
                exchangeDbContext
            );
        }
    }
}
