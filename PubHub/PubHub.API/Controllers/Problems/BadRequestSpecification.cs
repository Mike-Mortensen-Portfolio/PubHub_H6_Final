namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// Defines a standard <strong>Bad Request</strong> error specification that complies with the <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> standard
    /// </summary>
    internal static class BadRequestSpecification
    {
        public const int STATUS_CODE = StatusCodes.Status400BadRequest;
        public const string TITLE = "The request was malformed and cannot be processed";
    }
}
