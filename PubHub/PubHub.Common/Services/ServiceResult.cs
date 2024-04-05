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

        public bool IsSuccess => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);
        public HttpStatusCode StatusCode { get; }
        public string ErrorDescriptor { get; }

    }
}
