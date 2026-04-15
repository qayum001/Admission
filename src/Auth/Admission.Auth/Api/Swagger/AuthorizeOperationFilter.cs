using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Admission.Auth.Api.Swagger;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var methodInfo = context.MethodInfo;
        var controllerType = methodInfo.DeclaringType;

        var hasAllowAnonymous =
            endpointMetadata.OfType<IAllowAnonymous>().Any() ||
            methodInfo.GetCustomAttributes(inherit: true).OfType<IAllowAnonymous>().Any() ||
            (controllerType?.GetCustomAttributes(inherit: true).OfType<IAllowAnonymous>().Any() ?? false);

        var hasAuthorize =
            endpointMetadata.OfType<IAuthorizeData>().Any() ||
            methodInfo.GetCustomAttributes(inherit: true).OfType<IAuthorizeData>().Any() ||
            (controllerType?.GetCustomAttributes(inherit: true).OfType<IAuthorizeData>().Any() ?? false);

        if (hasAllowAnonymous || !hasAuthorize)
        {
            operation.Security = [];
            return;
        }

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document, null)] = []
            }
        ];

        operation.Responses ??= [];
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        const string authNote = "Requires JWT Bearer token.";
        if (string.IsNullOrWhiteSpace(operation.Description))
        {
            operation.Description = authNote;
            return;
        }

        if (!operation.Description.Contains(authNote, StringComparison.OrdinalIgnoreCase))
        {
            operation.Description = $"{operation.Description}\n\n{authNote}";
        }
    }
}
