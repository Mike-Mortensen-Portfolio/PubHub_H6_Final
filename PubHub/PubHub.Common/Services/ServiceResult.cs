using System.Net;

namespace PubHub.Common.Services
{
    public class ServiceResult
    {     
        internal ServiceResult(string? errorDescriptor) 
        {
            ErrorDescriptor = errorDescriptor;
        }

        public virtual bool IsSuccess => string.IsNullOrWhiteSpace(ErrorDescriptor);

        public string? ErrorDescriptor { get; }

    }
}
