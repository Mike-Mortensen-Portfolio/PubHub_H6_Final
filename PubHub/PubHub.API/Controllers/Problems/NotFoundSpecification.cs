namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// Defines a standard <strong>Not Found</strong> error specification that complies with the <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> standard
    /// </summary>
    internal static class NotFoundSpecification
    {
        public const int STATUS_CODE = StatusCodes.Status404NotFound;
        public const string TITLE = "The ressource you're looking for does not exist";
    }
}
