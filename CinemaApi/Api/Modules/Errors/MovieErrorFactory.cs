using Application.Movies.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MovieErrorFactory
{
    public static ObjectResult ToObjectResult(this MovieException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                MovieAlreadyExistsException => StatusCodes.Status409Conflict,
                MovieNotFoundException => StatusCodes.Status404NotFound,
                MovieUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Movie error handler is not implemented.")
            }
        };
    }
}