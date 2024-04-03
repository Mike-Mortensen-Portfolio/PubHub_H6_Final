using PubHub.Common.Services;

namespace PubHub.Common.ApiService
{
    public abstract class ServiceRoot
    {
        protected IHttpClientService Client { get; }
        protected string ClientName { get; }

        /// <summary>
        /// Holds a reference to the <see cref="IHttpClientService"/> with a <see cref="HttpClient"/> instance to use in the services.
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="clientName"></param>
        /// <exception cref="NullReferenceException"></exception>
        internal ServiceRoot(IHttpClientService clientService, string clientName)
        {
            Client = clientService;
            ClientName = clientName;

            if (Client == null)
                throw new NullReferenceException($"No HttpClient with name: {clientName} was found. Did you forget to add the service?");
        }
    }
}
