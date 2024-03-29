using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.ApiService
{
    public abstract class ServiceRoot
    {
        /// <summary>
        /// Use this to create new <see cref="HttpClient"/> <see langword="objects"/>
        /// </summary>
        protected IHttpClientFactory ClientFactory { get; }
        protected string ClientName { get; }

        protected HttpClient Client { get; }

        /// <summary>
        /// Creating the Client instance to use in the services.
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="clientName"></param>
        /// <exception cref="NullReferenceException"></exception>
        internal ServiceRoot(IHttpClientFactory clientFactory, string clientName)
        {
            ClientFactory = clientFactory;
            ClientName = clientName;

            Client = clientFactory.CreateClient(clientName);

            if (Client == null)
                throw new NullReferenceException($"No HttpClient with name: {clientName} was found. Did you forget to add the service?");
        }
    }
}
