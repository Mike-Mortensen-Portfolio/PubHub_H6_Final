using Polly;

namespace PubHub.Common.Services
{
    public interface IChaosService
    {
        /// <summary>
        /// Master switch for chaos injection.
        /// </summary>
        public bool Enabled { get; set; }
        public bool FaultEnabled { get; set; }
        public bool LatencyEnabled { get; set; }
        public bool OutcomeEnabled { get; set; }

        public double FaultInjectionRate { get; set; }
        public double LatencyInjectionRate { get; set; }
        public double OutcomeInjectionRate { get; set; }

        public double LatencySeconds { get; set; }

        public ValueTask<bool> IsChaosEnabledAsync(ResilienceContext context);
        public ValueTask<bool> IsFaultEnabledAsync(ResilienceContext context);
        public ValueTask<bool> IsLatencyEnabledAsync(ResilienceContext context);
        public ValueTask<bool> IsOutcomeEnabledAsync(ResilienceContext context);

        public ValueTask<double> GetFaultInjectionRateAsync(ResilienceContext context);
        public ValueTask<double> GetLatencyInjectionRateAsync(ResilienceContext context);
        public ValueTask<double> GetOutcomeInjectionRateAsync(ResilienceContext context);

        public ValueTask<Exception?> GetFaultAsync(ResilienceContext context);
        public ValueTask<TimeSpan> GetLatencyAsync(ResilienceContext context);
        public ValueTask<Outcome<HttpResponseMessage>?> GetOutcomeAsync(ResilienceContext context);
    }
}
