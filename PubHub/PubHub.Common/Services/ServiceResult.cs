﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Services
{
    public class ServiceResult 
    {     
        internal ServiceResult(HttpStatusCode statusCode, string errorDescriptor) 
        {
            StatusCode = statusCode;
            ErrorDescriptor = errorDescriptor;
        }

        public HttpStatusCode StatusCode { get; }
        public string ErrorDescriptor { get; }
    }
}
