using Polly;

namespace PubHub.Common.Services
{
    internal class ChaosService : IChaosService
    {
        public ChaosService() { }

        public ValueTask<double> GetInjectionRateAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(0.1); // 10%
        }

        public ValueTask<bool> IsChaosEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(true);
        }
    }
}
