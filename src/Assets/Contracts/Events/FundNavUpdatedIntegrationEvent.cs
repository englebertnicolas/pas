namespace PAS.Assets.Contracts.Events;

public record FundNavUpdatedIntegrationEvent(
    string FundIsin,
    string FundCurrency,
    DateTime NavDate,
    double NavOldValue,
    double NavNewValue
);
