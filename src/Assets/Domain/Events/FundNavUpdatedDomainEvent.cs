namespace PAS.Assets.Domain.Events;

public record FundNavUpdatedDomainEvent(
    string FundIsin,
    string FundCurrency,
    DateTime NavDate,
    double NavOldValue,
    double NavNewValue
) : IDomainEvent;
