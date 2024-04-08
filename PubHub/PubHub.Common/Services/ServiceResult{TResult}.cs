using System.Net;

namespace PubHub.Common.Services
{
    public class ServiceResult<TResult> : ServiceResult
    {
        internal ServiceResult(HttpStatusCode statusCode, TResult? instance, string errorDescriptor) : base(statusCode, errorDescriptor)
        {
            Instance = instance;
        }

        public TResult? Instance { get; }
    }
}
