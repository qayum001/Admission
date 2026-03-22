using System.Text.Json;
using Dictionary.Integration;

namespace Admission.Dictionary.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException e)
        {
            context.Response.StatusCode = e.StatusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "API_Error",
                statusCode = e.StatusCode,
                message = e.Response,
            };
            
            var json = JsonSerializer.Serialize(response);
            
            await context.Response.WriteAsync(json);
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 502;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(e.Message);
        }
        
    }
}