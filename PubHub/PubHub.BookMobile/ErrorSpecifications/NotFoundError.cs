using PubHub.Common;

namespace PubHub.BookMobile.ErrorSpecifications
{
    internal static class NotFoundError
    {
        public const string TITLE = "Missing Information";
        public const string ERROR_MESSAGE = $"Some information is missing. Sorry. Please try again, or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.NOT_FOUND}";
        public const string BUTTON_TEXT = "OK";
    }
}
