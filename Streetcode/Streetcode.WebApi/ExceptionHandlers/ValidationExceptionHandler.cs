using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Streetcode.WebApi.ExceptionHandlers;

public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        logger.LogError("Exception occured: {exception}", exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Extensions = { { nameof(validationException.Errors), validationException!.Errors } },
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
