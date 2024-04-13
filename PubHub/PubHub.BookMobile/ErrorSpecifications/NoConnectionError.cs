using PubHub.Common;

namespace PubHub.BookMobile.ErrorSpecifications
{
    internal static class NoConnectionError
    {
        public const string TITLE = "Error";
        public const string ERROR_MESSAGE = $"Couldn't establish a connection. Please try again, or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.NO_CONNECTION}";
        public const string BUTTON_TEXT = "OK";
    }
}
