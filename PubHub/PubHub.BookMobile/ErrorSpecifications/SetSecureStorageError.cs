using PubHub.Common;

namespace PubHub.BookMobile.ErrorSpecifications
{
    internal static class SetSecureStorageError
    {
        public const string TITLE = "Error";
        public const string ERROR_MESSAGE = $"Couldn't processs action. Please try again, or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.COULDNT_SET_SECURE_STORAGE}";
        public const string BUTTON_TEXT = "OK";
    }
}
