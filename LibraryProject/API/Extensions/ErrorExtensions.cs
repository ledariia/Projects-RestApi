using Application.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ErrorExtensions
{
    public static IActionResult ToActionResult(this BookError error)
    {
        return error switch
        {
            BookNotFoundError => new NotFoundObjectResult(new { Error = error.Message }),
            BookAlreadyExistsError => new ConflictObjectResult(new { Error = error.Message }),
            _ => new ObjectResult(new { Error = error.Message }) { StatusCode = 500 }
        };
    }
}