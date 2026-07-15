namespace PAS.Assets.Application.Funds.Queries;

public record FundQueryResult(
    int Id,
    string Name,
    string Isin,
    string Type,
    string Status
);

public class FundQueryHandler(IFundQueries queries) {

    public Task<FundQueryResult?> Handle(FundQuery request, CancellationToken cancellationToken) {
        return queries.GetFundAsync(request, cancellationToken);
    }
}
