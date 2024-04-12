using System.Net;

namespace PubHub.Common.Services
{
    public class ServiceResult
    {     
        internal ServiceResult(HttpStatusCode statusCode, string errorDescriptor) 
        {
            StatusCode = statusCode;
            ErrorDescriptor = errorDescriptor;
        }

        public bool IsSuccess => (StatusCode >= HttpStatusCode.OK) && (StatusCode <= HttpStatusCode.IMUsed);

        public HttpStatusCode StatusCode { get; }
        public string ErrorDescriptor { get; }

    }
}
