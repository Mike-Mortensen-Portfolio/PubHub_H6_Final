namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// Defines a standard <strong>Duplication</strong> error specification that complies with the <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> standard
    /// </summary>
    internal static class DuplicateProblemSpecification
    {
        public const string TYPE = "https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/wiki/Api-Documentation#duplicate-problem-specification";
        public const int STATUS_CODE = StatusCodes.Status409Conflict;
        public const string TITLE = "The resource is already present in persistence";
    }
}
