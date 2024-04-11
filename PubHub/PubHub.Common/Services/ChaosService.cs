using Polly;

namespace PubHub.Common.Services
{
    public class ChaosService : IChaosService
    {
        private double _faultInjectionRate = 0.1; // 10%

        public ChaosService() { }

        public bool Enabled { get; set; }
        public bool FaultInjectionEnabled { get; set; }

        public double FaultInjectionRate
        {
            get => _faultInjectionRate;
            set => SetInjectionRate(out _faultInjectionRate, value);
        }

        #region Enabled Generators
        public ValueTask<bool> IsChaosEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(Enabled);
        }

        public ValueTask<bool> IsFaultInjectionEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(FaultInjectionEnabled);
        }
        #endregion

        #region Injection Rate Generators
        public ValueTask<double> GetFaultInjectionRateAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(FaultInjectionRate);
        }
        #endregion

        private static void SetInjectionRate(out double field, double input) =>
            field = Math.Max(0, Math.Min(input, 1));
    }
}
