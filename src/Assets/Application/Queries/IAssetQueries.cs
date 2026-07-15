using PAS.Common.Application.Queries;

namespace PAS.Assets.Application.Queries;

public interface IAssetQueries {
    Task<PagedResult<CurrencyListQueryItemResult>> GetCurrencyListAsync(CurrencyListQuery request, CancellationToken cancellationToken);

    Task<PagedResult<FundListQueryItemResult>> GetFundListAsync(FundListQuery request, CancellationToken cancellationToken);
    Task<FundQueryResult?> GetFundAsync(FundQuery request, CancellationToken cancellationToken);
}
