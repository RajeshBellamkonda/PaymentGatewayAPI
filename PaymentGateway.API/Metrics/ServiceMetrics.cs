using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;

namespace PaymentGateway.API.Metrics
{
    public class ServiceMetrics : IServiceMetrics
    {
        private const string Application_Prefix = "payment_gateway_api";

        public ServiceMetrics(IMetrics metrics)
        {
            this.Metrics = metrics;
            this.ResponseTime = new TimerOptions
            {
                Name = $"{Application_Prefix}_response_time",
                MeasurementUnit = Unit.Requests,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            };

            this.HitCount = new CounterOptions
            {
                Name = $"{Application_Prefix}_hit_count",
                MeasurementUnit = Unit.Calls,
            };
        }

        /// <inheritdoc/>
        public IMetrics Metrics { get; set; }
        
        /// <inheritdoc/>
        public TimerOptions ResponseTime { get; set; }

        /// <inheritdoc/>
        public CounterOptions HitCount { get; }
    }
}
