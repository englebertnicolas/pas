using PAS.Assets.Application.Funds.Queries;
using PAS.Common.Application.Queries;

namespace PAS.Assets.Application;

public interface IAssetQueries {
    Task<PagedResult<FundListQueryItemResult>> GetFundListAsync(FundListQuery request, CancellationToken cancellationToken);
    Task<FundQueryResult?> GetFundAsync(FundQuery request, CancellationToken cancellationToken);
}
