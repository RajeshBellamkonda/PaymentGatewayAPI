using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using App.Metrics.Formatters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace PaymentGateway.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IMetricsRoot metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            return Host.CreateDefaultBuilder(args)
                 .UseSerilog()
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
