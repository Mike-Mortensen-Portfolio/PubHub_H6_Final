namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// Defines a standard <strong>Unauthorized/strong> error specification that complies with the <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> standard
    /// </summary>
    public class UnauthorizedSpecification
    {
        public const int STATUS_CODE = StatusCodes.Status401Unauthorized;
        public const string TITLE = "Authentication failed";
    }
}
