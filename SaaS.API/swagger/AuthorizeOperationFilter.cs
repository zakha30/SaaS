using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SaaS.Api.Swagger;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check [Authorize] on the action method
        var methodHasAuthorize = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any();

        // Check [Authorize] on the controller class
        var controllerHasAuthorize = context.MethodInfo
            .DeclaringType?
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any() ?? false;

        // Check [AllowAnonymous] on the action — overrides controller-level [Authorize]
        var hasAllowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        var requiresAuth = (methodHasAuthorize || controllerHasAuthorize)
                           && !hasAllowAnonymous;

        if (!requiresAuth)
            return;

        // Add the lock icon and Bearer requirement to this operation in Swagger UI
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name      = "Bearer",
                        In        = ParameterLocation.Header,
                        Type      = SecuritySchemeType.Http,
                        Scheme    = "bearer"
                    },
                    Array.Empty<string>()
                }
            }
        };

        // Add 401 response documentation to every secured endpoint
        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Unauthorized — JWT token missing or invalid."
            });
        }

        // Add 403 response documentation to every secured endpoint
        if (!operation.Responses.ContainsKey("403"))
        {
            operation.Responses.Add("403", new OpenApiResponse
            {
                Description = "Forbidden — insufficient role or cross-tenant access."
            });
        }
    }
}