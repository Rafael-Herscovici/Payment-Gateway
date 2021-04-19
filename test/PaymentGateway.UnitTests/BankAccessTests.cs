using System;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PaymentGatewayAPI.Services;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Common.Models;
using Newtonsoft.Json;
using Xunit;

namespace PaymentGateway.UnitTests
{
    public class BankAccessTests
    {
        private readonly Mock<ILogger<BankAccess>> _mockLogger =
            new Mock<ILogger<BankAccess>>();
        private readonly Mock<HttpMessageHandler> _mockMessageHandler =
            new Mock<HttpMessageHandler>();
        private readonly Fixture _fixture =
            new Fixture();

        [Fact]
        public async Task ProcessPaymentAsync_WithHappyPath_ShouldCompleteSuccessfullyAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            var paymentRequest = _fixture.Create<PaymentRequest>();

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(PaymentStatus.Success)),
                });

            var client = new HttpClient(_mockMessageHandler.Object) { BaseAddress = new Uri("http://somefakeaddress.com") };
            var sut = new BankAccess(client, _mockLogger.Object);

            // Act
            var result = await sut.ProcessPaymentAsync(paymentRequest, cancellationToken);

            // Assert
            Assert.Equal(PaymentStatus.Success, result);
            _mockMessageHandler.VerifyAll();
        }

        [Fact]
        public async Task ProcessPaymentAsync_WhenHttpClientFails_ShouldThrowExceptionAsync()
        {
            // Arrange
            CancellationToken cancellationToken = default;
            var paymentRequest = _fixture.Create<PaymentRequest>();
            var expectedException = new Exception("Something went wrong...");

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(expectedException);

            var client = new HttpClient(_mockMessageHandler.Object) { BaseAddress = new Uri("http://somefakeaddress.com") };
            var sut = new BankAccess(client, _mockLogger.Object);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.ProcessPaymentAsync(paymentRequest, cancellationToken));

            // Assert
            Assert.Equal(expectedException, ex);
            _mockMessageHandler.VerifyAll();
        }
    }
}
