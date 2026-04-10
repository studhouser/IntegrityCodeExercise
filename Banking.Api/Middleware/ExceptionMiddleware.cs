using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        context.Response.Headers.TryGetValue("X-Correlation-Id", out var correlationId);

        var problem = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "An unexpected error occurred",
            Detail = _env.IsDevelopment() ? exception.Message : "Please contact support with the provided Correlation ID.",
            Instance = context.Request.Path
        };

        problem.Extensions.Add("correlationId", correlationId.ToString());
        
        if (_env.IsDevelopment())
        {
            problem.Extensions.Add("stackTrace", exception.StackTrace);
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(problem, options);

        await context.Response.WriteAsync(json);
    }
}