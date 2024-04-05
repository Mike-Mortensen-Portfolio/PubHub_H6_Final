using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace PubHub.Common.Services
{
    public class ServiceResult<TResult> : ServiceResult
    {
        internal ServiceResult(HttpStatusCode statusCode, TResult? instance, string errorDescriptor) : base(statusCode, errorDescriptor)
        {
            Instance = instance;
        }

        [MemberNotNullWhen(true, nameof(IsSuccess))]
        public TResult? Instance { get; }
    }
}
