using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
