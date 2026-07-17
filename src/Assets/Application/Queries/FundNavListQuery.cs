using PAS.Common.Application.Queries;

namespace PAS.Assets.Application.Queries;

public record FundNavListQuery(
    long FundId,
    int PageNumber,
    int PageSize = 100,
    bool OrderAsc = false
) : IPagedQuery {

    public record Result(IReadOnlyCollection<ItemResult> Items, bool HasNextPage);
    public record ItemResult(DateTime Date, double Value);

    // Query validator (auto discovered by Wolverine)
    public class Validator : PagedQueryValidator<FundNavListQuery>;

    // Query handler (auto discovered by Wolverine)
    public static class Handler {
        public static Task<Result> HandleAsync(
            FundNavListQuery request,
            IAssetQueries queries,
            CancellationToken ct
        ) {
            return queries.GetFundNavListAsync(request, ct);
        }
    }
}
