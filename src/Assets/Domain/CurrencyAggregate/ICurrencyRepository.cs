namespace PAS.Assets.Domain.CurrencyAggregate;

public interface ICurrencyRepository : IRepository<Currency> {
    Currency Add(Currency currency);

    Task<bool> ExistsAsync(CurrencyId id, CancellationToken ct);
}
