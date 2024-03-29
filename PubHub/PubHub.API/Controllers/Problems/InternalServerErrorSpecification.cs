namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// Defines a standard <strong>Internal Server Error</strong> error specification that complies with the <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> standard
    /// </summary>
    internal static class InternalServerErrorSpecification
    {
        public const int STATUS_CODE = StatusCodes.Status500InternalServerError;
        public const string TITLE = "Our internal servers had some troubles processing the request";
    }
}
