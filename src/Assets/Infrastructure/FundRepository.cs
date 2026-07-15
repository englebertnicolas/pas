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

    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken) {
        return dbContext.Funds
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public Task<bool> IsinExistsAsync(Isin isin, CancellationToken cancellationToken) {
        return dbContext.Funds
            .AsNoTracking()
            .AnyAsync(x => x.Isin == isin, cancellationToken);
    }

    public async Task<Fund?> GetByNameWithRecentNavsAsync(string name, DateTime fromDate, CancellationToken cancellationToken) {
        var fund = await dbContext.Funds.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (fund != null)
            await LoadRecentNavsAsync(fund, fromDate, cancellationToken);

        return fund;
    }

    public async Task<Fund?> GetByIsinWithRecentNavsAsync(Isin isin, DateTime fromDate, CancellationToken cancellationToken) {
        var fund = await dbContext.Funds.SingleOrDefaultAsync(x => x.Isin == isin, cancellationToken);
        if (fund != null)
            await LoadRecentNavsAsync(fund, fromDate, cancellationToken);

        return fund;
    }

    private Task LoadRecentNavsAsync(Fund fund, DateTime fromDate, CancellationToken cancellationToken) {
        return dbContext.Funds
            .Where(f => f.Id == fund.Id)
            .Include(f => f.Navs.Where(x => x.Date >= fromDate))
            .LoadAsync(cancellationToken);
    }
}
