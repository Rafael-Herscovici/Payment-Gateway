using System;
using Common.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PaymentGatewayAPI.Services
{
    /// <summary>
    /// An interface defining the methods expected from a bank api to have
    /// </summary>
    public interface IBankAccess
    {
        /// <summary>
        /// Send a request to the bank to charge the user
        /// </summary>
        /// <param name="paymentRequest">A <see cref="PaymentRequest"/></param>
        /// <returns>A <see cref="PaymentStatus"/> indicating the result of trying to charge the account.</returns>
        Task<PaymentStatus> ProcessPaymentAsync(PaymentRequest paymentRequest);
    }

    /// <summary>
    /// A typed HttpClientFactory allowing communication with the bank api
    /// </summary>
    public class BankAccess : IBankAccess
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BankAccess> _logger;

        /// <inheritdoc cref="BankAccess"/>
        public BankAccess(
            HttpClient httpClient,
            ILogger<BankAccess> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PaymentStatus> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(paymentRequest));
            try
            {
                var result = await _httpClient.PostAsync("/api/transaction", stringContent);
                result.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<PaymentStatus>(await result.Content.ReadAsStringAsync());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to communicate with the bank.");
                throw;
            }
        }
    }
}
