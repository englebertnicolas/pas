using PAS.Assets.Domain.Events;

namespace PAS.Assets.Domain.FundAggregate;

public class Fund : Entity, IAggregateRoot {
    public FundType Type { get; private set; }
    public FundStatus Status { get; private set; }
    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public Currency Currency { get; private set; } = null!;

    private readonly List<FundNav> navs = [];
    public IReadOnlyCollection<FundNav> Navs => navs.AsReadOnly();

    private Fund() {
        // Required for Entity Framework Core hydration
    }

    private Fund(FundType type, FundStatus status, string name, Isin isin, Currency currency, IEnumerable<FundNav>? navs = null) {
        Type = type;
        Status = status;
        Name = name;
        Isin = isin;
        Currency = currency;
        if (navs != null) this.navs = [.. navs];
    }

    public static Fund CreateCollective(FundStatus status, string name, Isin isin, Currency currency, IEnumerable<FundNav>? navs = null) {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Invalid fund name.");
        return new Fund(FundType.Collective, status, name, isin, currency, navs);
    }

    public static Fund CreateDedicated(FundStatus status, string name, Isin isin, Currency currency, IEnumerable<FundNav>? navs = null) {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Invalid fund name.");
        return new Fund(FundType.Dedicated, status, name, isin, currency, navs);
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
            AddDomainEvent(new FundNavUpdatedDomainEvent(Isin.Value, Currency.Code, nav.Date, existingNav.Value, nav.Value));
    }

    public void SetClosedStatus() {
        if (Status == FundStatus.Closed)
            throw new DomainException($"Cannot change fund status from '{Status}' to '{FundStatus.Closed}'.");

        Status = FundStatus.Closed;

        AddDomainEvent(new FundStatusChangedToClosedDomainEvent(this));
    }
}
