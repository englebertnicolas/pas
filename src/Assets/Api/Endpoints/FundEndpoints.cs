using Microsoft.AspNetCore.Http.HttpResults;
using PAS.Assets.Application.Funds.Commands;
using PAS.Assets.Application.Funds.Queries;
using PAS.Common.Application.Queries;
using Wolverine;

namespace PAS.Assets.Api.Endpoints;

public static class FundEndpoints {

    public static IEndpointRouteBuilder MapFundEndpoints(this IEndpointRouteBuilder builder) {
        var funds = builder.MapGroup("/funds");

        funds.MapGet("", GetFundListAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("GetFundList");

        funds.MapPost("/search", SearchFundsAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("SearchFunds");

        funds.MapGet("{id:int}", GetFundAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("GetFund");

        funds.MapPost("/collective", CreateCollectiveFundAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("CreateCollectiveFund");

        funds.MapPost("/dedicated", () => TypedResults.StatusCode(501)) // Not implemented
            .ProducesProblem(501)
            .WithName("CreateDedicatedFund");

        funds.MapPut("navs", AddOrUpdateFundNavAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("AddOrUpdateFundNav");

        return builder;
    }

    public static async Task<Ok<PagedResult<FundListQueryItemResult>>> GetFundListAsync(IMessageBus bus, int pageNumber = 1, int pageSize = 100) {
        var req = new FundListQuery(pageNumber, pageSize);
        var resp = await bus.InvokeAsync<PagedResult<FundListQueryItemResult>>(req);
        return TypedResults.Ok(resp);
    }

    public static async Task<Ok<PagedResult<FundListQueryItemResult>>> SearchFundsAsync(IMessageBus bus, FundListQuery request) {
        var resp = await bus.InvokeAsync<PagedResult<FundListQueryItemResult>>(request);
        return TypedResults.Ok(resp);
    }

    public static async Task<Results<Ok<FundQueryResult>, NotFound>> GetFundAsync(IMessageBus bus, int id) {
        var req = FundQuery.ById(id);
        var resp = await bus.InvokeAsync<FundQueryResult>(req);
        if (resp == null) return TypedResults.NotFound();
        return TypedResults.Ok(resp);
    }

    public static async Task<Created<CreateCollectiveFundCommandResult>> CreateCollectiveFundAsync(IMessageBus bus, CreateCollectiveFundCommand request) {
        var resp = await bus.InvokeAsync<CreateCollectiveFundCommandResult>(request);
        return TypedResults.Created($"/funds/{resp.Id}", resp);
    }

    public static async Task<Ok> AddOrUpdateFundNavAsync(IMessageBus bus, AddOrUpdateFundNavCommand request) {
        await bus.InvokeAsync(request);
        return TypedResults.Ok();
    }
}
