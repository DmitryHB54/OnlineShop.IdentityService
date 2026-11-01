using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.IdentityService.Application.Common.Exceptions;

namespace OnlineShop.IdentityService.WebApi.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(Logger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblem(validationException),
            UnauthorizedException unauthorizedException => CreateProblem(401, "Unauthorized", unauthorizedException.Message, "rfc7235"),
            NotFoundException nfe => CreateProblem(404, "Not found", nfe.Message, "rfc7231#section-6.5.4"),
            InvalidOperationException ioe => CreateProblem(400, "Bad request", ioe.Message, "rfc7231#section-6.5.1"),
            _  => CreateProblem(500, "Unexpected error", null, "rfc7231-6.6.1")
        };

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = problemDetails.Status ?? 500;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateValidationProblem(ValidationException validationException)
    {
        Dictionary<string, string[]> errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ProblemDetails
        {
            Status = 400,
            Title = "One or more validation errors occured.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
        problem.Extensions["errors"] = errors;
        
        return problem;
    }

    private static ProblemDetails CreateProblem(int status, string title, string? detail, string typeSuffix)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://tools.ietf.org/html/{typeSuffix}"
        };
        return problem;
    }
}