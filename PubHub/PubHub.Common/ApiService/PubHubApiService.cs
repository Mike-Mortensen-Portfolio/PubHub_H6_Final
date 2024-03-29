using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using PubHub.Common.Services;

namespace PubHub.Common.ApiService
{
    public class PubHubApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        /// <summary>
        /// The name of the <see cref="HttpClient"/> used to generate calls against the <strong>PubHub</strong> API
        /// </summary>
        public string ClientName { get; }

        public UserService Users { get; }
        public BookService Books { get; }

        public PubHubApiService(IHttpClientFactory clientFactory, string clientName)
        {
            _clientFactory = clientFactory;
            ClientName = clientName;

            Users = new UserService(clientFactory, clientName);
            Books = new BookService(clientFactory, clientName);
        }
    }
}
