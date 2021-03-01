using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using App.Metrics.Formatters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PaymentGateway.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IMetricsRoot metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            return Host.CreateDefaultBuilder(args)
                 .ConfigureMetrics(metrics)
                 .ConfigureAppMetricsHostingConfiguration(options => { options.MetricsEndpoint = "/metrics"; })
                 .UseMetrics(
                    options =>
                    {
                        options.EndpointOptions = endpointOptions =>
                        {
                            endpointOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.GetType<MetricsPrometheusTextOutputFormatter>();
                        };
                    })  
                 .ConfigureWebHostDefaults(webBuilder =>
                 webBuilder.UseStartup<Startup>());
        }
    }
}
