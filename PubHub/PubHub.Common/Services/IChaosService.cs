using Polly;

namespace PubHub.Common.Services
{
    internal interface IChaosService
    {
        public ValueTask<double> GetInjectionRateAsync(ResilienceContext context);
        public ValueTask<bool> IsChaosEnabledAsync(ResilienceContext context);
    }
}
