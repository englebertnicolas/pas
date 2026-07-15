namespace PAS.Assets.Application.Commands;

public record CreateCurrencyCommand(
    string Id,
    string EnglishName,
    string? Symbol = null
);
