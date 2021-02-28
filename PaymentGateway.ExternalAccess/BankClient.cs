using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.ExternalAccess
{
    public class BankClient : IBankClient
    {
        private readonly ILogger<BankClient> _logger;
        private readonly IBankConfiguration _bankConfiguration;
        private string _token;
        private DateTime _tokenExpiry;

        private const string GetPaymentResourceUrl = "payment";
        private const string AcceptContent = "application/json";

        public BankClient(ILogger<BankClient> logger, IBankConfiguration bankConfiguration)
        {
            _logger = logger;
            _bankConfiguration = bankConfiguration;
        }

        private HttpClient GetHttpClient()
        {
            if (_token == null || _tokenExpiry == DateTime.MinValue || _tokenExpiry < DateTime.UtcNow)
            {
                _logger.LogDebug("Getting access token from the Bank");
                //TODO: get token and return
                _token = "121D29B8D4534AF086B313CB7F927726";
                _tokenExpiry = DateTime.UtcNow.AddSeconds(36000);
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(_bankConfiguration.BaseUrl)
            };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("bearer", _token);
            httpClient.DefaultRequestHeaders.Add("accept", AcceptContent);
            return httpClient;
        }

        public async Task<Payment> GetPaymentDetails(string id)
        {
            using var httpClient = GetHttpClient();
            var paymentDetailsResponse = await httpClient.GetAsync(string.Concat(GetPaymentResourceUrl, $"/{id}"));
            _logger.LogInformation($"Payment details status for {id}: {paymentDetailsResponse.StatusCode}");
            if (paymentDetailsResponse.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Payment>(await paymentDetailsResponse.Content.ReadAsStringAsync());
            }
            return default(Payment);
        }

        public async Task<Payment> ProcessPayment(Payment payment)
        {
            using var httpClient = GetHttpClient();
            var paymentResponse = await httpClient.PostAsync(GetPaymentResourceUrl,
                new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, AcceptContent));
            _logger.LogInformation($"Payment details status for {payment.MaskedCardNumber}: {paymentResponse.StatusCode}");
            paymentResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Payment>(await paymentResponse.Content.ReadAsStringAsync());
        }
    }
}
