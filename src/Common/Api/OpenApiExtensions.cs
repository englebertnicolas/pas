using System.Text.Json.Serialization.Metadata;
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
        return serviceProvider.AddOpenApi(options => {
            options.CreateSchemaReferenceId = SchemaReferenceIdSelector;
            options
                .AddOperationTransformer(AddOperationResponse500Async)
                .AddOperationTransformer(ConvertOperationParametersToCamelCaseAsync);
        });
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

    private static string? SchemaReferenceIdSelector(JsonTypeInfo jsonTypeInfo) {
        var defaultId = OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
        if (defaultId is null) return null;
        return GetCleanTypeNameForCommandsAndQueries(jsonTypeInfo.Type);
    }

    /// <summary>
    /// Generates a clean type name for nested types of Command/Query models.
    /// E.g.: Nested type CreateFundCommand+Request -> CreateFundCommandRequest
    /// </summary>
    private static string GetCleanTypeNameForCommandsAndQueries(Type type) {
        if (type.IsNested && type.DeclaringType != null) {
            if (type.DeclaringType.Name.EndsWith("Command") || type.DeclaringType.Name.EndsWith("Query")) {
                return $"{type.DeclaringType.Name}{type.Name}";
            }
        }

        // Generic types should also have a clean name
        // E.g.: PagedQueryResult<FundListQuery+ItemResult> -> PagedQueryResultOfFundListQueryItemResult
        if (type.IsGenericType) {
            var baseName = type.GetGenericTypeDefinition().Name;
            var backtickIndex = baseName.IndexOf('`');
            if (backtickIndex > 0) baseName = baseName[..backtickIndex];

            var arguments = type.GetGenericArguments().Select(GetCleanTypeNameForCommandsAndQueries);
            return $"{baseName}Of{string.Join("And", arguments)}";
        }

        return type.Name;
    }

    /// <summary>
    /// Adds a 500 operation response with ProblemDetails schema.
    /// </summary>
    private static async Task AddOperationResponse500Async(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken ct) {
        var internalErrorResponse = new OpenApiResponse {
            Description = "Internal Server Error",
            Content = new Dictionary<string, OpenApiMediaType>(),
        };
        internalErrorResponse.Content.Add("application/problem+json", new OpenApiMediaType {
            Schema = await context.GetOrCreateSchemaAsync(typeof(ProblemDetails), null, ct)
        });

        operation.Responses ??= [];
        operation.Responses.TryAdd("500", internalErrorResponse);
    }

    /// <summary>
    /// Converts the operation parameter names to camelCase to match the default JSON serialization naming policy.
    /// </summary>
    private static Task ConvertOperationParametersToCamelCaseAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken ct) {
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
