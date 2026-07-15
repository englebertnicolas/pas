namespace PAS.Assets.Domain.FundAggregate;

public interface IFundRepository : IRepository<Fund> {
    Fund Add(Fund fund);
    void Update(Fund fund);

    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    Task<bool> IsinExistsAsync(Isin isin, CancellationToken cancellationToken);

    Task<Fund?> GetByIdWithRecentNavsAsync(long id, DateTime fromDate, CancellationToken cancellationToken);
    Task<Fund?> GetByNameWithRecentNavsAsync(string name, DateTime fromDate, CancellationToken cancellationToken);
    Task<Fund?> GetByIsinWithRecentNavsAsync(Isin isin, DateTime fromDate, CancellationToken cancellationToken);
}
