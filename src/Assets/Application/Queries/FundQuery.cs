using FluentValidation;

namespace PAS.Assets.Application.Queries;

public record FundQuery {
    public long? Id { get; init;}
    public string? Name { get; init; }
    public string? Isin { get; init; }

    private FundQuery(long? id, string? name, string? isin) {
        Id = id;
        Name = name;
        Isin = isin;
    }

    public static FundQuery ById(long id) => new(id, null, null);
    public static FundQuery ByName(string name) => new(null, name, null);
    public static FundQuery ByIsin(string isin) => new(null, null, isin);

    public record Result(
        long Id,
        string Name,
        string Isin,
        string Type,
        string Status
    );

    // Query validator (auto discovered by Wolverine)
    public class Validator : AbstractValidator<FundQuery> {
        public Validator() {
            RuleFor(x => x)
                .Must(x => (x.Id != null ? 1 : 0)
                         + (!string.IsNullOrEmpty(x.Name) ? 1 : 0)
                         + (!string.IsNullOrEmpty(x.Isin) ? 1 : 0) == 1)
                .WithMessage("One and only one of the following must be supplied: Id, Name, or Isin.");

            RuleFor(x => x.Isin)
                .Length(12)
                .When(x => !string.IsNullOrEmpty(x.Isin));
        }
    }

    // Query handler (auto discovered by Wolverine)
    public static class Handler {
        public static Task<Result?> HandleAsync(
            FundQuery request,
            IAssetQueries queries,
            CancellationToken ct
        ) {
            return queries.GetFundAsync(request, ct);
        }
    }
}
