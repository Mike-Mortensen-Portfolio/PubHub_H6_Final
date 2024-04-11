﻿using Polly;

namespace PubHub.Common.Services
{
    public interface IChaosService
    {
        /// <summary>
        /// Master switch for chaos injection.
        /// </summary>
        public bool Enabled { get; set; }
        public bool FaultInjectionEnabled { get; set; }

        public double FaultInjectionRate { get; set; }

        public ValueTask<bool> IsChaosEnabledAsync(ResilienceContext context);
        public ValueTask<bool> IsFaultInjectionEnabledAsync(ResilienceContext context);

        public ValueTask<double> GetFaultInjectionRateAsync(ResilienceContext context);
    }
}
