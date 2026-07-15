using Microsoft.EntityFrameworkCore;
using PAS.Assets.Application.Queries;
using PAS.Common.Application.Queries;

namespace PAS.Assets.Infrastructure;

public class AssetQueries(AssetDbContext dbContext) : IAssetQueries {

    public async Task<PagedResult<CurrencyListQueryItemResult>> GetCurrencyListAsync(CurrencyListQuery request, CancellationToken cancellationToken) {
        var items = await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize + 1)
            .Select(x => new CurrencyListQueryItemResult(
                x.Id.Value,
                x.EnglishName,
                x.Symbol.Value
            ))
            .ToListAsync(cancellationToken);

        bool hasNextPage = items.Count > request.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }

    public Task<FundQueryResult?> GetFundAsync(FundQuery request, CancellationToken cancellationToken) {
        return dbContext.Funds
            .AsNoTracking()
            .WhereIf(request.Id != null, x => x.Id == request.Id)
            .WhereIf(request.Isin != null, x => x.Isin.Value == request.Isin)
            .WhereIf(request.Name != null, x => x.Name == request.Name)
            .Select(x => new FundQueryResult(
                x.Id,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString()
            ))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<FundListQueryItemResult>> GetFundListAsync(FundListQuery request, CancellationToken cancellationToken) {
        var items = await dbContext.Funds
            .AsNoTracking()
            .WhereSearch(x => x.Name, request.NameSearch)
            .OrderBy(x => x.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize + 1)
            .Select(x => new FundListQueryItemResult(
                x.Id,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString()
            ))
            .ToListAsync(cancellationToken);

        bool hasNextPage = items.Count > request.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }
}
