using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Services
{
    public class ServiceResult<T>
    {     
        internal ServiceResult(HttpStatusCode statusCode, T instance, string errorDescriptor) 
        {
            StatusCode = statusCode;
            Instance = instance;
            ErrorDescriptor = errorDescriptor;
        }

        public HttpStatusCode StatusCode { get; }
        public T Instance { get; }
        public string ErrorDescriptor { get; }

    }
}
