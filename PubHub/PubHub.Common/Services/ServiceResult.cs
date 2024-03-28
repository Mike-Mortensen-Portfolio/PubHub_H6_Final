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
        internal ServiceResult(string statusCode, T instance, string errorDescriptor) 
        {
            this.StatusCode = statusCode;
            this.Instance = instance;
            this.ErrorDescriptor = errorDescriptor;
        }

        public string StatusCode { get; }
        public T Instance { get; }
        public string ErrorDescriptor { get; }

    }
}
