using System.Net;
using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Converte exceções comuns em respostas JSON no formato <c>.doc/general-api.md</c> (type, error, detail).
/// </summary>
public sealed class DocumentedExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly RequestDelegate _next;

    public DocumentedExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new DocumentedErrorResponse
                {
                    Type = "ValidationError",
                    Error = "Invalid input data",
                    Detail = string.Join(" ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")),
                });
        }
        catch (KeyNotFoundException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.NotFound,
                new DocumentedErrorResponse
                {
                    Type = "ResourceNotFound",
                    Error = "Resource not found",
                    Detail = ex.Message,
                });
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.Unauthorized,
                new DocumentedErrorResponse
                {
                    Type = "AuthenticationError",
                    Error = "Invalid authentication token",
                    Detail = ex.Message,
                });
        }
        catch (InvalidOperationException ex)
        {
            await WriteAsync(
                context,
                HttpStatusCode.BadRequest,
                new DocumentedErrorResponse
                {
                    Type = "BusinessRuleViolation",
                    Error = "Request cannot be completed",
                    Detail = ex.Message,
                });
        }
    }

    private static Task WriteAsync(HttpContext context, HttpStatusCode status, DocumentedErrorResponse body)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
