using System.Net;

namespace PubHub.Common.Services
{
    public class ServiceResult<TResult> : ServiceResult
    {
        internal ServiceResult(TResult? instance, string? errorDescriptor) : base(errorDescriptor)
        {
            Instance = instance;
        }

        public TResult? Instance { get; }
    }
}
