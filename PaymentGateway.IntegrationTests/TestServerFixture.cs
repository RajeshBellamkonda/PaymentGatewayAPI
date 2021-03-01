using App.Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;

namespace PaymentGateway.IntegrationTests
{
    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }

        public TestServerFixture()
        {
            IMetricsRoot metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

            var builder = new WebHostBuilder()
                .ConfigureMetrics(metrics)
                .UseEnvironment("Development")
                .UseStartup<TestStartup>();

            this._testServer = new TestServer(builder);
            this.Client = this._testServer.CreateClient();

        }

        public void Dispose()
        {
            this.Dispose(true);
            this._testServer?.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Client?.Dispose();
                this._testServer?.Dispose();
            }
        }


    }
}
