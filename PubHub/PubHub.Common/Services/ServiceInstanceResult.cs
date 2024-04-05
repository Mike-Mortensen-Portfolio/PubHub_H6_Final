using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace PubHub.Common.Services
{
    public class ServiceInstanceResult<T> : ServiceResult
    {
        internal ServiceInstanceResult(HttpStatusCode statusCode, T? instance, string errorDescriptor) : base(statusCode, errorDescriptor)
        {
            Instance = instance;
        }

        [MemberNotNullWhen(true, nameof(IsSuccess))]
        public T? Instance { get; }
    }
}
