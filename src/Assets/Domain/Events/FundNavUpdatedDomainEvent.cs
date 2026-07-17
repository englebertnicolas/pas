namespace PAS.Assets.Domain.Events;

public record FundNavUpdatedDomainEvent(
    long FundId,
    string FundIsin,
    string FundCurrency,
    DateTime NavDate,
    double NavOldValue,
    double NavNewValue
) : IDomainEvent;
