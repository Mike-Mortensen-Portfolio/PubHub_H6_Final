namespace PubHub.API.Controllers.Problems
{
    internal static class TooManyRequestsSpecification
    {
        public const int STATUS_CODE = StatusCodes.Status429TooManyRequests;
        public const string TITLE = "Too many requests than allowed. Please wait.";
    }
}
