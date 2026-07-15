namespace PAS.Assets.Application.Commands;

public record AddOrUpdateFundNavCommand(
    long FundId,
    DateTime NavDate,
    double NavValue
);
