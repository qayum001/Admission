using System.Net;
using System.Text.Json;
using Admission.Application.Exceptions;
using Admission.Domain.Exceptions;

namespace Admission.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            NotFoundException => ((int)HttpStatusCode.NotFound, "Not Found"),
            InvalidActionException => ((int)HttpStatusCode.BadRequest, "Invalid Action"),
            InvalidDataDomainException => ((int)HttpStatusCode.BadRequest, "Invalid Domain Data"),
            DomainException => ((int)HttpStatusCode.UnprocessableEntity, "Domain Error"),
            _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = new
        {
            title,
            status = statusCode,
            detail = exception.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
