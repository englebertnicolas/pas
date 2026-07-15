using PAS.Common.Application.Queries;

namespace PAS.Assets.Application.Queries;

public class CurrencyListQueryHandler(IAssetQueries queries) {

    public Task<PagedResult<CurrencyListQueryItemResult>> Handle(CurrencyListQuery request, CancellationToken cancellationToken) {
        return queries.GetCurrencyListAsync(request, cancellationToken);
    }
}

public record CurrencyListQueryItemResult(
    string Id,
    string EnglishName,
    string Symbol
);
