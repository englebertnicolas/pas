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
}
