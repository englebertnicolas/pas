namespace PAS.Assets.Application.Commands;

public record CreateCollectiveFundCommand(
    string Name,
    string Isin,
    string Currency,
    CreateFundCommandNavInput? Nav = null
);

public record CreateFundCommandNavInput(DateTime Date, double Value);
