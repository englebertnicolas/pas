using PAS.Common.Application.Queries;

namespace PAS.Assets.Application.Funds.Queries;

public class FundListQueryHandler(IAssetQueries queries) {

    public Task<PagedResult<FundListQueryItemResult>> Handle(FundListQuery request, CancellationToken cancellationToken) {
        return queries.GetFundListAsync(request, cancellationToken);
    }
}

public record FundListQueryItemResult(
    string Name,
    string Isin,
    string Type,
    string Status
);
