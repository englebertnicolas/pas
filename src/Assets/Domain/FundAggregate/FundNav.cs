namespace PAS.Assets.Domain.FundAggregate;

public record FundNav : ValueObject {
    public DateTime Date { get; private set; }
    public double Value { get; private set; }

    private FundNav(DateTime date, double value) {
        Date = date;
        Value = value;
    }

    public static FundNav Create(DateTime date, double value) {
        if (date < new DateTime(1900, 1, 1) || date > DateTime.Now) 
            throw new DomainException("Invalid NAV date.");
        if (value <= 0)
            throw new DomainException("NAV must be greater than zero.");

        return new FundNav(date, value);
    }
}
