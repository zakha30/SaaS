using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SaaS.API.Swagger;

/// <summary>
/// Fixes Swagger generation for file upload endpoints with [FromForm] IFormFile
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if this endpoint has [Consumes("multipart/form-data")]
        var isMultipartFormData = context.MethodInfo
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ConsumesAttribute), false)
            .Cast<Microsoft.AspNetCore.Mvc.ConsumesAttribute>()
            .Any(attr => attr.ContentTypes.Any(ct => ct.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase)));

        if (!isMultipartFormData)
            return;

        // Clear the default request body
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    "multipart/form-data",
                    new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                {
                                    "file",
                                    new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary",
                                        Description = "The image file (JPG, PNG, WEBP, GIF - max 5MB)"
                                    }
                                }
                            },
                            Required = new HashSet<string> { "file" }
                        }
                    }
                }
            }
        };

        // Remove the file parameter from the parameters list
        operation.Parameters = operation.Parameters
            .Where(p => p.Name != "file")
            .ToList();
    }
}