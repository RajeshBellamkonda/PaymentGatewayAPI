using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;

namespace PaymentGateway.API.Metrics
{
    public interface IServiceMetrics
    {
        IMetrics Metrics { get; set; }

        /// <summary>
        /// Used from Reponse tracking Middleware to track response times
        /// </summary>
        TimerOptions ResponseTime { get; set; }

        /// <summary>
        /// Used to track total number of times an endpoint has been hit
        /// </summary>
        CounterOptions HitCount { get; }
    }
}
