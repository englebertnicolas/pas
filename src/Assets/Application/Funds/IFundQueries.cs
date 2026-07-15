using PAS.Assets.Application.Funds.Queries;
using PAS.Common.Application.Queries;

namespace PAS.Assets.Application.Funds;

public interface IFundQueries {
    Task<PagedResult<FundListQueryItemResult>> GetFundListAsync(FundListQuery request, CancellationToken cancellationToken);
    Task<FundQueryResult?> GetFundAsync(FundQuery request, CancellationToken cancellationToken);
}
