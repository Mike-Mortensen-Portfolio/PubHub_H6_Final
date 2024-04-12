using PubHub.Common;

namespace PubHub.BookMobile.ErrorSpecifications
{
    internal static class UnprocessableEntityError
    {
        public const string TITLE = "Bad Format";
        public const string ERROR_MESSAGE = $"We couldn't processs your action. Try again or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.UNPROCESSABLE_ENTITY}";
        public const string BUTTON_TEXT = "OK";
    }
}
