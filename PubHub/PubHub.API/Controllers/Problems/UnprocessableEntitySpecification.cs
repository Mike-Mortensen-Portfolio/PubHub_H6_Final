namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// Defines a standard <strong>Unprocessable Entity</strong> error specification that complies with the <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> standard
    /// </summary>
    public class UnprocessableEntitySpecification
    {
        public const int STATUS_CODE = StatusCodes.Status422UnprocessableEntity;
        public const string TITLE = "The entity couldn't be processed because one or more criteria wasn't met";
    }
}
