namespace PAS.Assets.Application.Queries;

public interface IAssetQueries {
    Task<CurrencyListQuery.Result> GetCurrencyListAsync(CurrencyListQuery request, CancellationToken ct);

    Task<FundListQuery.Result> GetFundListAsync(FundListQuery request, CancellationToken ct);
    Task<FundQuery.Result?> GetFundAsync(FundQuery request, CancellationToken ct);
    Task<FundNavListQuery.Result> GetFundNavListAsync(FundNavListQuery request, CancellationToken ct);
}
