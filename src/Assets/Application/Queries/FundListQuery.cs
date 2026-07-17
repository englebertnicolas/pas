using PAS.Common.Application.Queries;

namespace PAS.Assets.Application.Queries;

public record FundListQuery(
    int PageNumber = 1,
    int PageSize = 100,
    string? NameSearch = null
) : IPagedQuery {

    public record Result(IReadOnlyCollection<ItemResult> Items, bool HasNextPage);
    public record ItemResult(
        long Id,
        string Name,
        string Isin,
        string Type,
        string Status
    );

    // Query validator (auto discovered by Wolverine)
    public class Validator : PagedQueryValidator<FundListQuery>;

    // Query handler (auto discovered by Wolverine)
    public static class Handler {
        public static Task<Result> HandleAsync(
            FundListQuery request,
            IAssetQueries queries,
            CancellationToken ct
        ) {
            return queries.GetFundListAsync(request, ct);
        }
    }
}
