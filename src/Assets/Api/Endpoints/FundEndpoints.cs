using Microsoft.AspNetCore.Http.HttpResults;
using PAS.Assets.Application.Commands;
using PAS.Assets.Application.Queries;
using Wolverine;

namespace PAS.Assets.Api.Endpoints;

public static class FundEndpoints {

    public static IEndpointRouteBuilder MapFundEndpoints(this IEndpointRouteBuilder builder) {
        var funds = builder.MapGroup("/funds");

        funds.MapGet("", GetFundListAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("GetFundList");

        funds.MapPost("search", SearchFundsAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("SearchFunds");

        funds.MapGet("{id:long}", GetFundAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("GetFund");

        funds.MapPost("collective", CreateCollectiveFundAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("CreateCollectiveFund");

        funds.MapPost("dedicated", CreateDedicatedFundAsync)
            .ProducesProblem(501)
            .WithName("CreateDedicatedFund");

        funds.MapGet("{id:long}/navs", GetFundNavListAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("GetFundNavList");

        funds.MapPut("{id:long}/navs", AddOrUpdateFundNavAsync)
            .ProducesValidationProblem(400)
            .ProducesValidationProblem(422)
            .WithName("AddOrUpdateFundNav");

        return builder;
    }

    public static async Task<Ok<FundListQuery.Result>> GetFundListAsync(IMessageBus bus, int pageNumber = 1, int pageSize = 100) {
        var query = new FundListQuery(pageNumber, pageSize);
        var resp = await bus.InvokeAsync<FundListQuery.Result>(query);
        return TypedResults.Ok(resp);
    }

    public static async Task<Ok<FundListQuery.Result>> SearchFundsAsync(IMessageBus bus, FundListQuery request) {
        var resp = await bus.InvokeAsync<FundListQuery.Result>(request);
        return TypedResults.Ok(resp);
    }

    public static async Task<Results<Ok<FundQuery.Result>, NotFound>> GetFundAsync(IMessageBus bus, long id) {
        var query = FundQuery.ById(id);
        var resp = await bus.InvokeAsync<FundQuery.Result>(query);
        if (resp == null) return TypedResults.NotFound();
        return TypedResults.Ok(resp);
    }

    public static async Task<Created<CreateCollectiveFundCommand.Result>> CreateCollectiveFundAsync(IMessageBus bus, CreateCollectiveFundCommand request) {
        var resp = await bus.InvokeAsync<CreateCollectiveFundCommand.Result>(request);
        return TypedResults.Created($"/funds/{resp.Id}", resp);
    }

    public static async Task<IResult> CreateDedicatedFundAsync() {
        return TypedResults.StatusCode(501);
    }

    public static async Task<Ok<FundNavListQuery.Result>> GetFundNavListAsync(IMessageBus bus, long id, int pageNumber = 1, int pageSize = 100, bool orderAsc = false) {
        var query = new FundNavListQuery(id, pageNumber, pageSize, orderAsc);
        var resp = await bus.InvokeAsync<FundNavListQuery.Result>(query);
        return TypedResults.Ok(resp);
    }

    public static async Task<Ok> AddOrUpdateFundNavAsync(IMessageBus bus, long id, AddOrUpdateFundNavBody request) {
        var command = new AddOrUpdateFundNavCommand(id, request.Date, request.Value);
        await bus.InvokeAsync(command);
        return TypedResults.Ok();
    }

    public record AddOrUpdateFundNavBody(DateTime Date, double Value);
}
