namespace PAS.Assets.Application.Funds.Queries;

public record FundQuery {
    public int? Id { get; init;}
    public string? Name { get; init; }
    public string? Isin { get; init; }

    private FundQuery(int? id, string? name, string? isin) {
        Id = id;
        Name = name;
        Isin = isin;
    }

    public static FundQuery ById(int id) => new(id, null, null);
    public static FundQuery ByName(string name) => new(null, name, null);
    public static FundQuery ByIsin(string isin) => new(null, null, isin);
}
