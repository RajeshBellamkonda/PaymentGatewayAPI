using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Client.Configuration;
using PaymentGateway.Client.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Client
{
    public class PaymentGatewayClient : IPaymentGatewayClient
    {
        private readonly ILogger<PaymentGatewayClient> _logger;
        private readonly IPaymentGatewayConfiguration _paymentGatewayConfiguration;
        private string _token;
        private DateTime _tokenExpiry;

        private const string GetPaymentResourceUrl = "payment";
        private const string AcceptContent = "application/json";

        public PaymentGatewayClient(ILogger<PaymentGatewayClient> logger, IPaymentGatewayConfiguration paymentGatewayConfiguration)
        {
            _logger = logger;
            _paymentGatewayConfiguration = paymentGatewayConfiguration;
        }

        private async Task<HttpClient> GetHttpClient()
        {
            if (_token == null || _tokenExpiry == DateTime.MinValue || _tokenExpiry < DateTime.UtcNow)
            {
                //_logger.LogDebug("Getting access token from the Bank");
                using var client = new HttpClient();
                try
                {
                    var disco = await client.GetDiscoveryDocumentAsync(_paymentGatewayConfiguration.AuthServerUrl);

                    if (disco.IsError)
                    {
                        _logger.LogError(disco.Error);
                        throw new Exception(disco.Error);
                    }

                    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = _paymentGatewayConfiguration.ClientId,
                        ClientSecret = _paymentGatewayConfiguration.ClientSecret,
                        Scope = _paymentGatewayConfiguration.Scope
                    });

                    if (tokenResponse.IsError)
                    {
                        _logger.LogError(tokenResponse.Error);
                        throw new Exception(tokenResponse.Error);
                    }
                    _token = tokenResponse.AccessToken;
                    _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw ex;
                }
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(_paymentGatewayConfiguration.BaseUrl)
            };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.SetBearerToken(_token);
            httpClient.DefaultRequestHeaders.Add("accept", AcceptContent);
            return httpClient;
        }

        /// <summary>
        /// Gets Payment details.
        /// </summary>
        /// <param name="id">Payment Id.</param>
        /// <returns>PaymentDetails.</returns>
        public async Task<PaymentSummary> GetPaymentDetails(string id)
        {
            using var httpClient = await GetHttpClient();
            var paymentDetailsResponse = await httpClient.GetAsync(string.Concat(GetPaymentResourceUrl, $"/{id}"));
            _logger.LogInformation($"Payment details status for {id}: {paymentDetailsResponse.StatusCode}");
            paymentDetailsResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<PaymentSummary>(await paymentDetailsResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Processes payments through the PaymentsGateway.
        /// </summary>
        /// <param name="payment">PaymentRequest with payment details.</param>
        /// <returns>Payment Sumary after processing the payment.</returns>
        public async Task<PaymentSummary> ProcessPayment(PaymentRequest payment)
        {
            using var httpClient = await GetHttpClient();
            var paymentResponse = await httpClient.PostAsync(GetPaymentResourceUrl,
                new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, AcceptContent));
            _logger.LogInformation($"Payment details status: {paymentResponse.StatusCode}");
            paymentResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<PaymentSummary>(await paymentResponse.Content.ReadAsStringAsync());
        }
    }
}
