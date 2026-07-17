using Microsoft.EntityFrameworkCore;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Infrastructure;

public class FundRepository(AssetDbContext dbContext) : IFundRepository {

    public Fund Add(Fund fund) {
        return dbContext.Funds.Add(fund).Entity;
    }

    public void Update(Fund fund) {
        dbContext.Entry(fund).State = EntityState.Modified;
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct) {
        return dbContext.Funds
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, ct);
    }

    public Task<bool> IsinExistsAsync(Isin isin, CancellationToken ct) {
        return dbContext.Funds
            .AsNoTracking()
            .AnyAsync(x => x.Isin == isin, ct);
    }

    public async Task<Fund?> GetByIdWithRecentNavsAsync(long id, DateTime fromDate, CancellationToken ct) {
        var fund = await dbContext.Funds.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (fund != null)
            await LoadRecentNavsAsync(fund, fromDate, ct);

        return fund;
    }

    public async Task<Fund?> GetByNameWithRecentNavsAsync(string name, DateTime fromDate, CancellationToken ct) {
        var fund = await dbContext.Funds.SingleOrDefaultAsync(x => x.Name == name, ct);
        if (fund != null)
            await LoadRecentNavsAsync(fund, fromDate, ct);

        return fund;
    }

    public async Task<Fund?> GetByIsinWithRecentNavsAsync(Isin isin, DateTime fromDate, CancellationToken ct) {
        var fund = await dbContext.Funds.SingleOrDefaultAsync(x => x.Isin == isin, ct);
        if (fund != null)
            await LoadRecentNavsAsync(fund, fromDate, ct);

        return fund;
    }

    private Task LoadRecentNavsAsync(Fund fund, DateTime fromDate, CancellationToken ct) {
        return dbContext.Funds
            .Where(f => f.Id == fund.Id)
            .Include(f => f.Navs.Where(x => x.Date >= fromDate))
            .LoadAsync(ct);
    }
}
