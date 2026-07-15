namespace PAS.Assets.Application.Funds.Queries;

public record FundListQuery(
    int PageNumber,
    int PageSize = 100
) {
    public string? NameSearch { get; init; }
}
