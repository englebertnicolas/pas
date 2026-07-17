using Microsoft.EntityFrameworkCore;
using PAS.Assets.Application.Queries;

namespace PAS.Assets.Infrastructure;

public class AssetQueries(AssetDbContext dbContext) : IAssetQueries {

    public async Task<CurrencyListQuery.Result> GetCurrencyListAsync(CurrencyListQuery request, CancellationToken ct) {
        var items = await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize + 1)
            .Select(x => new CurrencyListQuery.ItemResult(
                x.Id.Value,
                x.EnglishName,
                x.Symbol.Value
            ))
            .ToListAsync(ct);

        bool hasNextPage = items.Count > request.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }

    public Task<FundQuery.Result?> GetFundAsync(FundQuery request, CancellationToken ct) {
        return dbContext.Funds
            .AsNoTracking()
            .WhereIf(request.Id != null, x => x.Id == request.Id)
            .WhereIf(request.Isin != null, x => x.Isin.Value == request.Isin)
            .WhereIf(request.Name != null, x => x.Name == request.Name)
            .Select(x => new FundQuery.Result(
                x.Id,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString()
            ))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<FundListQuery.Result> GetFundListAsync(FundListQuery request, CancellationToken ct) {
        var items = await dbContext.Funds
            .AsNoTracking()
            .WhereSearch(x => x.Name, request.NameSearch)
            .OrderBy(x => x.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize + 1)
            .Select(x => new FundListQuery.ItemResult(
                x.Id,
                x.Name,
                x.Isin.Value,
                x.Type.ToString(),
                x.Status.ToString()
            ))
            .ToListAsync(ct);

        bool hasNextPage = items.Count > request.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }

    public async Task<FundNavListQuery.Result> GetFundNavListAsync(FundNavListQuery request, CancellationToken ct) {
        var items = await dbContext.Funds
            .AsNoTracking()
            .Where(x => x.Id == request.FundId)
            .SelectMany(x => x.Navs)
            .OrderBy(request.OrderAsc, x => x.Date)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize + 1)
            .Select(x => new FundNavListQuery.ItemResult(
                x.Date,
                x.Value
            ))
            .ToListAsync(ct);

        bool hasNextPage = items.Count > request.PageSize;
        items = hasNextPage ? [.. items.SkipLast(1)] : items;

        return new(items, hasNextPage);
    }
}
