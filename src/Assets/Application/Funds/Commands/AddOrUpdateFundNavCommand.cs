namespace PAS.Assets.Application.Funds.Commands;

public record AddOrUpdateFundNavCommand(
    string Isin,
    DateTime NavDate,
    double NavValue
);
