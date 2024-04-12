﻿using Polly;

namespace PubHub.Common.Services
{
    public class ChaosService : IChaosService
    {
        // 0 is 0% and 1 is 100%.
        private double _faultInjectionRate = 0.1;
        private double _latencyInjectionRate = 0.1;
        private double _outcomeInjectionRate = 0.1;

        private double _latencySeconds = 10;

        public bool Enabled { get; set; }
        public bool FaultEnabled { get; set; }
        public bool LatencyEnabled { get; set; }
        public bool OutcomeEnabled { get; set; }

        public double LatencySeconds
        {
            get => _latencySeconds;
            set => _latencySeconds = value;
        }

        public double FaultInjectionRate
        {
            get => _faultInjectionRate;
            set => SetInjectionRate(out _faultInjectionRate, value);
        }
        public double LatencyInjectionRate
        {
            get => _latencyInjectionRate;
            set => SetInjectionRate(out _latencyInjectionRate, value);
        }
        public double OutcomeInjectionRate
        {
            get => _outcomeInjectionRate;
            set => SetInjectionRate(out _outcomeInjectionRate, value);
        }

        #region Enabled Generators
        public ValueTask<bool> IsChaosEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(Enabled);
        }

        public ValueTask<bool> IsFaultEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(Enabled && FaultEnabled);
        }

        public ValueTask<bool> IsLatencyEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(Enabled && LatencyEnabled);
        }

        public ValueTask<bool> IsOutcomeEnabledAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(Enabled && OutcomeEnabled);
        }
        #endregion

        #region Injection Rate Generators
        public ValueTask<double> GetFaultInjectionRateAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(FaultInjectionRate);
        }

        public ValueTask<double> GetLatencyInjectionRateAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(LatencyInjectionRate);
        }

        public ValueTask<double> GetOutcomeInjectionRateAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(OutcomeInjectionRate);
        }
        #endregion

        #region Value Generators
        public ValueTask<TimeSpan> GetLatencyAsync(ResilienceContext context)
        {
            return ValueTask.FromResult(TimeSpan.FromSeconds(LatencySeconds));
        }
        #endregion

        private static void SetInjectionRate(out double field, double input) =>
            field = Math.Max(0, Math.Min(input, 1));
    }
}
