using System.Net;

namespace PubHub.Common.Services
{
    public class HttpServiceResult<TResult> : HttpServiceResult
    {
        internal HttpServiceResult(HttpStatusCode statusCode, TResult? instance, string? errorDescriptor) : base(statusCode, errorDescriptor)
        {
            Instance = instance;
        }

        public TResult? Instance { get; }
    }
}
