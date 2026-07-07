using Hearth.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hearth.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
        => result.IsSuccess
            ? new OkObjectResult(result.Value)
            : Problem(result.Error!);

    public static IActionResult ToActionResult(this Result result)
        => result.IsSuccess
            ? new NoContentResult()
            : Problem(result.Error!);

    private static IActionResult Problem(Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Status = status
        };

        return new ObjectResult(problem) { StatusCode = status };
    }
}
