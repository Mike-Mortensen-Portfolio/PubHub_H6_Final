using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Services
{
    public class HttpServiceResult : ServiceResult
    {
        internal HttpServiceResult(HttpStatusCode statusCode, string? errorDescriptor) : base(errorDescriptor)
        {
            StatusCode = statusCode;
        }

        public override bool IsSuccess => (StatusCode >= HttpStatusCode.OK) && (StatusCode <= HttpStatusCode.IMUsed);

        public HttpStatusCode StatusCode { get; }
    }
}
