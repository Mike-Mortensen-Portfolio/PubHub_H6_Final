using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.ApiService
{
    public class ApiOptions
    {
        /// <summary>
        /// The base address used to configure the <see langword="internal"/> <see cref="HttpClient"/>
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// The name of the <see langword="internal"/> <see cref="HttpClient"/>
        /// </summary>
        public string? HttpClientName { get; set; }
    }
}
