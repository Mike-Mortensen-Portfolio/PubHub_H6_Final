namespace PubHub.Common.ErrorSpecifications
{
    public static class ConflictError
    {
        public const string TITLE = "Conflicting values";
        public const string ERROR_MESSAGE = $"We couldn't processs your action. Try again or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.CONFLICT}";
        public const string BUTTON_TEXT = "OK";
    }
}
