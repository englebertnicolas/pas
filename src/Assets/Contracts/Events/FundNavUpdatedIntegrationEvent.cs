namespace PAS.Assets.Contracts.Events;

public record FundNavUpdatedIntegrationEvent(
    long FundId,
    string FundIsin,
    string FundCurrency,
    DateTime NavDate,
    double NavOldValue,
    double NavNewValue
);
