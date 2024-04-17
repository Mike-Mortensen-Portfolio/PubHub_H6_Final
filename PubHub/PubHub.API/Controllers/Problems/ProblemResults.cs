namespace PubHub.API.Controllers.Problems
{
    /// <summary>
    /// A factory for <see cref="IResult"/> with <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>.
    /// </summary>
    public static class ProblemResults
    {
        public static IResult UnauthorizedResult() => Results.Problem(
            statusCode: UnauthorizedSpecification.STATUS_CODE,
            title: UnauthorizedSpecification.TITLE,
            detail: "Unauthorized access to resource.");
    }
}
