using Microsoft.EntityFrameworkCore;
using PAS.Assets.Domain.CurrencyAggregate;

namespace PAS.Assets.Infrastructure;

public class CurrencyRepository(AssetDbContext dbContext) : ICurrencyRepository {

    public Currency Add(Currency currency) {
        return dbContext.Currencies.Add(currency).Entity;
    }

    public Task<bool> ExistsAsync(CurrencyId id, CancellationToken cancellationToken) {
        return dbContext.Currencies
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}
