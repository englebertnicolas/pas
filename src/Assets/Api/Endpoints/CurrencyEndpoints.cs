using Microsoft.AspNetCore.Http.HttpResults;
using PAS.Assets.Application.Commands;
using PAS.Assets.Application.Queries;
using Wolverine;

namespace PAS.Assets.Api.Endpoints;

public static class CurrencyEndpoints {

    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder builder) {
        var funds = builder.MapGroup("/currencies");

        funds.MapGet("", GetCurrencyListAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("GetCurrencyList");

        funds.MapPost("", CreateCurrencyAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("CreateCurrency");

        return builder;
    }

    public static async Task<Ok<CurrencyListQuery.Result>> GetCurrencyListAsync(IMessageBus bus, int pageNumber = 1, int pageSize = 100) {
        var req = new CurrencyListQuery(pageNumber, pageSize);
        var resp = await bus.InvokeAsync<CurrencyListQuery.Result>(req);
        return TypedResults.Ok(resp);
    }

    public static async Task<Created<CreateCurrencyCommand.Result>> CreateCurrencyAsync(IMessageBus bus, CreateCurrencyCommand request) {
        var resp = await bus.InvokeAsync<CreateCurrencyCommand.Result>(request);
        return TypedResults.Created($"/currencies/{resp.Id}", resp);
    }
}
