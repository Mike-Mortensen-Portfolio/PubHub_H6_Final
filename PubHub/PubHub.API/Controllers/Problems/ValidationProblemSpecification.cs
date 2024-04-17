namespace PubHub.API.Controllers.Problems
{
    internal static class ValidationProblemSpecification
    {
        public const string TYPE = "https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/wiki/Api-Documentation#validation-problem-specification";
        public const int STATUS_CODE = StatusCodes.Status400BadRequest;
        public const string TITLE = "One or more properties were invalid.";
    }
}
