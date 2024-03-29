using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Services
{
    public class ServiceInstanceResult<T>
    {
        internal ServiceInstanceResult(HttpStatusCode statusCode, T instance, string errorDescriptior) 
        {
            StatusCode = statusCode;
            Instance = instance;
            ErrorDescriptor = errorDescriptior;
        }

        public HttpStatusCode StatusCode { get; }
        public T Instance { get; set; }
        public string ErrorDescriptor { get; }
    }
}
