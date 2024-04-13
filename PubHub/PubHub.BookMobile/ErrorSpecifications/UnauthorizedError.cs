using PubHub.Common;

namespace PubHub.BookMobile.ErrorSpecifications
{
    internal static class UnauthorizedError
    {
        public const string TITLE = "Access Denied";
        public const string ERROR_MESSAGE = $"You're not allowed to access this. If you think this is a mistake, try again or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.UNAUTHORIZED}";
        public const string BUTTON_TEXT = "Sorry";
    }
}
