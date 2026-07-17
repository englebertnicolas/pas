using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.Events;

namespace PAS.Assets.Domain.FundAggregate;

public class Fund : Entity, IAggregateRoot {
    public FundType Type { get; private set; }
    public FundStatus Status { get; private set; }
    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public CurrencyId CurrencyId { get; private set; } = null!;

    private readonly List<FundNav> navs = [];
    public IReadOnlyCollection<FundNav> Navs => navs.AsReadOnly();

    private Fund() {
        // For EF hydration
    }

    internal Fund(FundType type, FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        Type = type;
        Status = status;
        Name = name;
        Isin = isin;
        CurrencyId = currencyId;
        if (navs != null) this.navs = [.. navs];
    }

    public static Fund CreateCollectiveFund(FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        ValidateFundCreation(name, isin);
        return new(FundType.Collective, status, name, isin, currencyId, navs);
    }

    public static Fund CreateDedicatedFund(FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        ValidateFundCreation(name, isin);
        return new(FundType.Dedicated, status, name, isin, currencyId, navs);
    }

    private static void ValidateFundCreation(string name, Isin _) {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Invalid fund name.", nameof(Name));
    }

    public void AddOrUpdateNav(DateTime date, double value) => AddOrUpdateNav(FundNav.Create(date, value));
    public void AddOrUpdateNav(FundNav nav) {
        var existingNav = navs.FirstOrDefault(v => v.Date == nav.Date);
        if (existingNav != null) {
            if (existingNav.Value == nav.Value) return;
            navs.Remove(existingNav);
        }

        navs.Add(nav);

        if (existingNav != null)
            AddDomainEvent(new FundNavUpdatedDomainEvent(Id, Isin.Value, CurrencyId.Value, nav.Date, existingNav.Value, nav.Value));
    }

    public void SetClosedStatus() {
        if (Status == FundStatus.Closed)
            throw new DomainException($"Cannot change fund status from '{Status}' to '{FundStatus.Closed}'.", nameof(Status));

        Status = FundStatus.Closed;

        AddDomainEvent(new FundStatusChangedToClosedDomainEvent(this));
    }
}
