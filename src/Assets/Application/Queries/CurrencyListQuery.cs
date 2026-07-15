namespace PAS.Assets.Application.Queries;

public record CurrencyListQuery(
    int PageNumber,
    int PageSize = 100
);
