using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentGateway.Client.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentGateway.Client
{
    public class Program
    {
        static IConfiguration Configuration => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((_, services) =>
                   {
                       var paymentGatewayConfig = new PaymentGatewayConfiguration();
                       Configuration.GetSection("PaymentGateway").Bind(paymentGatewayConfig);
                       services.AddTransient<IPaymentGatewayClient, PaymentGatewayClient>();
                       services.AddSingleton<IPaymentGatewayConfiguration>(paymentGatewayConfig);
                   });

        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var paymentClient = provider.GetRequiredService<IPaymentGatewayClient>();

            // Application code should start here.
            var paymentId = Guid.NewGuid().ToString();
            var paymentSummary = await paymentClient.GetPaymentDetails(paymentId);
            Console.WriteLine($"Successfully Retrieved Payment for Id : {paymentId}");
            Console.WriteLine(JsonConvert.SerializeObject(paymentSummary, Formatting.Indented));
            Console.ReadLine();
        }
        public static async Task Main1(string[] args)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "PaymentGatewayAPI",
                ClientSecret = "DF84B468-31FB-493C-A56A-A69C34ED80CE",
                Scope = "pg-read pg-write"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:7702/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
            Console.ReadLine();
        }
    }
}
