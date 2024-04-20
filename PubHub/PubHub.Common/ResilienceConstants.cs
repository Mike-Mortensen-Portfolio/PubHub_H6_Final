namespace PubHub.Common
{
    internal class ResilienceConstants
    {
        public const int MAX_RETRY_ATTEMPTS = 5;

        public const string CONTENT_MEGABYTES_RESILIENCE_KEY = "size";
        public const string INFO_SERVICE_KEY = "info";
    }
}
