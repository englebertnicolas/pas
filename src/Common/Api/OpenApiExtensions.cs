using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace PAS.Common.Api;

public static class OpenApiExtensions {

    public static IServiceCollection AddDefaultOpenApi(this IServiceCollection serviceProvider) {
        return serviceProvider.AddOpenApi(options => options
            .AddOperationTransformer(AddOperationResponse500Async)
            .AddOperationTransformer(ConvertOperationParametersToCamelCaseAsync)
        );
    }

    public static WebApplication UseDefaultOpenApi(this WebApplication app, string? title = null) {
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
            app.MapScalarApiReference(options => { 
                if (!string.IsNullOrWhiteSpace(title))
                    options.Title = title;
            });
        }

        return app;
    }

    /// <summary>
    /// Adds a 500 operation response with ProblemDetails schema
    /// </summary>
    private static async Task AddOperationResponse500Async(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) {
        var internalErrorResponse = new OpenApiResponse {
            Description = "Internal Server Error",
            Content = new Dictionary<string, OpenApiMediaType>(),
        };
        internalErrorResponse.Content.Add("application/problem+json", new OpenApiMediaType {
            Schema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetails), null, cancellationToken)
        });

        operation.Responses ??= [];
        operation.Responses.TryAdd("500", internalErrorResponse);
    }

    /// <summary>
    /// Converts the operation parameter names to camelCase to match the default JSON serialization naming policy
    /// </summary>
    private static Task ConvertOperationParametersToCamelCaseAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken) {
        if (operation.Parameters != null) {
            foreach (var parameter in operation.Parameters) {
                if (parameter is OpenApiParameter concreteParameter && concreteParameter.Name != null) {
                    concreteParameter.Name = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(concreteParameter.Name);
                }
            }
        }
        return Task.CompletedTask;
    }
}
