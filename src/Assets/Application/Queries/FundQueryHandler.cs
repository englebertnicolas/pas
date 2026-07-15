namespace PAS.Assets.Application.Queries;

public record FundQueryResult(
    long Id,
    string Name,
    string Isin,
    string Type,
    string Status
);

public class FundQueryHandler(IAssetQueries queries) {

    public Task<FundQueryResult?> Handle(FundQuery request, CancellationToken cancellationToken) {
        return queries.GetFundAsync(request, cancellationToken);
    }
}
