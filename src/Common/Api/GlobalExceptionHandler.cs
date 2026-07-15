using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PAS.Common.Application;
using PAS.Common.Domain;

namespace PAS.Common.Api;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger, 
    IWebHostEnvironment env
) : IExceptionHandler {

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken) {

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        // Determine the problem properties based on the exception type
        var (statusCode, title, detail, errors) = exception switch {
            FluentValidation.ValidationException ex => (
                StatusCodes.Status400BadRequest,
                "Validation error",
                "The request payload contains invalid data.",
                ex.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            ),

            DomainException ex => (
                StatusCodes.Status422UnprocessableEntity,
                "Unprocessable request",
                "Business rule violation",
                new Dictionary<string, string[]> { { ex.PropertyName, [ex.Message] } }),

            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Item not found",
                ex.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                env.IsDevelopment() ? new Dictionary<string, string[]> { { "exception", [exception.ToString()] } } : null)
        };

        // Logging the exception
        if (statusCode >= StatusCodes.Status500InternalServerError) {
            logger.LogError(exception, "Internal serveur error [TraceId: {TraceId}]: {Message}", traceId, exception.Message);
        } else {
            logger.LogWarning("Request failed ({Status}) [TraceId: {TraceId}]: {Message}", statusCode, traceId, exception.Message);
        }

        // Buiding and writing response
        if (errors is not null) {
            var validationDetails = new ValidationProblemDetails(errors) {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            validationDetails.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(validationDetails, cancellationToken);

        } else {
            var problemDetails = new ProblemDetails {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }
}
